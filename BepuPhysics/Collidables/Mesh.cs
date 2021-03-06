﻿using BepuPhysics.CollisionDetection.CollisionTasks;
using BepuPhysics.Trees;
using BepuUtilities;
using BepuUtilities.Collections;
using BepuUtilities.Memory;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using Quaternion = BepuUtilities.Quaternion;
namespace BepuPhysics.Collidables
{
    public unsafe interface ICompoundRayHitHandler
    {
        void OnRayHit(int childIndex, float* maximumT, float t, in Vector3 normal);
    }

    public struct Mesh : IMeshShape
    {
        public Tree Tree;
        public Buffer<Triangle> Triangles;
        internal Vector3 scale;
        internal Vector3 inverseScale;
        public Vector3 Scale
        {
            get
            {
                return scale;
            }
            set
            {
                scale = value;
                inverseScale = new Vector3(
                    value.X != 0 ? 1f / value.X : float.MaxValue,
                    value.Y != 0 ? 1f / value.Y : float.MaxValue,
                    value.Z != 0 ? 1f / value.Z : float.MaxValue);
            }
        }

        public Mesh(Buffer<Triangle> triangles, Vector3 scale, BufferPool pool) : this()
        {
            Triangles = triangles;
            Tree = new Tree(pool, triangles.Length);
            pool.Take<BoundingBox>(triangles.Length, out var boundingBoxes);
            for (int i = 0; i < triangles.Length; ++i)
            {
                ref var t = ref triangles[i];
                ref var bounds = ref boundingBoxes[i];
                bounds.Min = Vector3.Min(t.A, Vector3.Min(t.B, t.C));
                bounds.Max = Vector3.Max(t.A, Vector3.Max(t.B, t.C));
            }
            Tree.SweepBuild(pool, boundingBoxes.Slice(0, triangles.Length));
            pool.Return(ref boundingBoxes);
            Scale = scale;
        }

        public int TriangleCount => Triangles.Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void GetLocalTriangle(int triangleIndex, out Triangle target)
        {
            ref var source = ref Triangles[triangleIndex];
            target.A = scale * source.A;
            target.B = scale * source.B;
            target.C = scale * source.C;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void GetLocalTriangle(int triangleIndex, ref TriangleWide target)
        {
            //This inserts a triangle into the first slot of the given wide instance.
            ref var source = ref Triangles[triangleIndex];
            Vector3Wide.WriteFirst(source.A, ref target.A);
            Vector3Wide.WriteFirst(source.B, ref target.B);
            Vector3Wide.WriteFirst(source.C, ref target.C);
        }

        public void ComputeBounds(in BepuUtilities.Quaternion orientation, out Vector3 min, out Vector3 max)
        {
            Matrix3x3.CreateFromQuaternion(orientation, out var r);
            min = new Vector3(float.MaxValue);
            max = new Vector3(-float.MaxValue);
            for (int i = 0; i < Triangles.Length; ++i)
            {
                //This isn't an ideal bounding box calculation for a mesh. 
                //-You might be able to get a win out of widely vectorizing.
                //-Indexed smooth meshes would tend to have a third as many max/min operations.
                //-Even better would be a set of extreme points that are known to fully enclose the mesh, eliminating the need to test the vast majority.
                //But optimizing this only makes sense if dynamic meshes are common, and they really, really, really should not be.
                ref var triangle = ref Triangles[i];
                Matrix3x3.Transform(scale * triangle.A, r, out var a);
                Matrix3x3.Transform(scale * triangle.B, r, out var b);
                Matrix3x3.Transform(scale * triangle.C, r, out var c);
                var min0 = Vector3.Min(a, b);
                var min1 = Vector3.Min(c, min);
                var max0 = Vector3.Max(a, b);
                var max1 = Vector3.Max(c, max);
                min = Vector3.Min(min0, min1);
                max = Vector3.Max(max0, max1);
            }
        }

        public ShapeBatch CreateShapeBatch(BufferPool pool, int initialCapacity, Shapes shapeBatches)
        {
            return new MeshShapeBatch<Mesh>(pool, initialCapacity);
        }

        unsafe struct FirstHitLeafTester : IRayLeafTester
        {
            Triangle* triangles;
            public float MinimumT;
            public Vector3 MinimumNormal;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public FirstHitLeafTester(in Buffer<Triangle> triangles)
            {
                this.triangles = (Triangle*)triangles.Memory;
                MinimumT = float.MaxValue;
                MinimumNormal = default;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public unsafe void TestLeaf(int leafIndex, RayData* rayData, float* maximumT)
            {
                ref var triangle = ref triangles[leafIndex];
                if (Triangle.RayTest(triangle.A, triangle.B, triangle.C, rayData->Origin, rayData->Direction, out var t, out var normal) && t < MinimumT && t <= *maximumT)
                {
                    MinimumT = t;
                    MinimumNormal = normal;
                }
            }
        }
        
        /// <summary>
        /// Casts a ray against the mesh. Returns the first hit.
        /// </summary>
        /// <param name="pose">Pose of the mesh during the ray test.</param>
        /// <param name="origin">Origin of the ray.</param>
        /// <param name="direction">Direction of the ray.</param>
        /// <param name="maximumT">Maximum length of the ray in units of the ray direction length.</param>
        /// <param name="t">First impact time in units of the ray direction length.</param>
        /// <param name="normal">First impact normal.</param>
        /// <returns>True if the ray hit anything, false otherwise.</returns>
        public bool RayTest(in RigidPose pose, in Vector3 origin, in Vector3 direction, float maximumT, out float t, out Vector3 normal)
        {
            Matrix3x3.CreateFromQuaternion(pose.Orientation, out var orientation);
            Matrix3x3.TransformTranspose(origin - pose.Position, orientation, out var localOrigin);
            Matrix3x3.TransformTranspose(direction, orientation, out var localDirection);
            localOrigin *= inverseScale;
            localDirection *= inverseScale;
            var leafTester = new FirstHitLeafTester(Triangles);
            Tree.RayCast(localOrigin, localDirection, maximumT, ref leafTester);
            if (leafTester.MinimumT < float.MaxValue)
            {
                t = leafTester.MinimumT;
                Matrix3x3.Transform(leafTester.MinimumNormal * inverseScale, orientation, out normal);
                normal = Vector3.Normalize(normal);
                return true;
            }
            t = default;
            normal = default;
            return false;
        }

        unsafe struct HitLeafTester<T> : IRayLeafTester where T : ICompoundRayHitHandler
        {
            Triangle* triangles;
            internal T HitHandler;
            Matrix3x3 orientation;
            Vector3 inverseScale;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public HitLeafTester(in Buffer<Triangle> triangles, in Matrix3x3 orientation, in Vector3 inverseScale, in T hitHandler)
            {
                this.triangles = (Triangle*)triangles.Memory;
                HitHandler = hitHandler;
                this.orientation = orientation;
                this.inverseScale = inverseScale;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public unsafe void TestLeaf(int leafIndex, RayData* rayData, float* maximumT)
            {
                ref var triangle = ref triangles[leafIndex];
                if (Triangle.RayTest(triangle.A, triangle.B, triangle.C, rayData->Origin, rayData->Direction, out var t, out var normal))
                {
                    //Pull the hit back into world space before handing it off to the user. This does cost a bit more, but not much, and you can always add a specialized no-transform path later.
                    Matrix3x3.Transform(normal * inverseScale, orientation, out normal);
                    normal = Vector3.Normalize(normal);
                    HitHandler.OnRayHit(leafIndex, maximumT, t, normal);
                }
            }
        }

        /// <summary>
        /// Casts a ray against the mesh. Executes a callback for every hit.
        /// </summary>
        /// <typeparam name="TRayHitHandler">Type of the callback to execute for every hit.</typeparam>
        /// <param name="pose">Pose of the mesh during the ray test.</param>
        /// <param name="origin">Origin of the ray.</param>
        /// <param name="direction">Direction of the ray.</param>
        /// <param name="maximumT">Maximum length of the ray in units of the ray direction length.</param>
        /// <param name="hitHandler">Callback to execute for every hit.</param>
        public void RayTest<TRayHitHandler>(in RigidPose pose, in Vector3 origin, in Vector3 direction, float maximumT, ref TRayHitHandler hitHandler) where TRayHitHandler : struct, ICompoundRayHitHandler
        {
            Matrix3x3.CreateFromQuaternion(pose.Orientation, out var orientation);
            Matrix3x3.TransformTranspose(origin - pose.Position, orientation, out var localOrigin);
            Matrix3x3.TransformTranspose(direction, orientation, out var localDirection);
            localOrigin *= inverseScale;
            localDirection *= inverseScale;
            var leafTester = new HitLeafTester<TRayHitHandler>(Triangles, orientation, inverseScale, hitHandler);
            Tree.RayCast(localOrigin, localDirection, maximumT, ref leafTester);
            //The leaf tester could have mutated the hit handler; copy it back over.
            hitHandler = leafTester.HitHandler;
        }

        /// <summary>
        /// Casts a bunch of rays against the tree at the same time, passing the time of first impact for each ray to the given callback.
        /// Used by RayBatcher.
        /// </summary>
        /// <typeparam name="TRayHitHandler">Type of the callback to execute for all ray first hits.</typeparam>
        /// <param name="pose">Pose of the mesh during the ray test.</param>
        /// <param name="rays">Set of rays to cast against the mesh.</param>
        /// <param name="hitHandler">Callback to execute for all ray first hits.</param>
        public unsafe void RayTest<TRayHitHandler>(in RigidPose pose, ref RaySource rays, ref TRayHitHandler hitHandler) where TRayHitHandler : struct, IShapeRayBatchHitHandler
        {
            //TODO: Note that we dispatch a bunch of scalar tests here. You could be more clever than this- batched tests are possible. 
            //May be worth creating a different traversal designed for low ray counts- might be able to get some benefit out of a semidynamic packet or something.
            Matrix3x3.CreateFromQuaternion(pose.Orientation, out var orientation);
            Matrix3x3.Transpose(orientation, out var inverseOrientation);
            var leafTester = new FirstHitLeafTester(Triangles);
            for (int i = 0; i < rays.RayCount; ++i)
            {
                rays.GetRay(i, out var ray, out var maximumT);
                Matrix3x3.Transform(ray->Origin - pose.Position, inverseOrientation, out var localOrigin);
                Matrix3x3.Transform(ray->Direction, inverseOrientation, out var localDirection);
                localOrigin *= inverseScale;
                localDirection *= inverseScale;
                leafTester.MinimumT = float.MaxValue;
                Tree.RayCast(localOrigin, localDirection, *maximumT, ref leafTester);
                if (leafTester.MinimumT < float.MaxValue)
                {
                    Matrix3x3.Transform(leafTester.MinimumNormal * inverseScale, orientation, out var normal);
                    normal = Vector3.Normalize(normal);
                    hitHandler.OnRayHit(i, leafTester.MinimumT, normal);
                }
            }
        }


        unsafe struct Enumerator<TSubpairOverlaps> : IBreakableForEach<int> where TSubpairOverlaps : ICollisionTaskSubpairOverlaps
        {
            public BufferPool Pool;
            public void* Overlaps;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool LoopBody(int i)
            {
                Unsafe.AsRef<TSubpairOverlaps>(Overlaps).Allocate(Pool) = i;
                return true;
            }
        }

        public unsafe void FindLocalOverlaps<TOverlaps, TSubpairOverlaps>(PairsToTestForOverlap* pairs, int count, BufferPool pool, Shapes shapes, ref TOverlaps overlaps)
            where TOverlaps : struct, ICollisionTaskOverlaps<TSubpairOverlaps>
            where TSubpairOverlaps : struct, ICollisionTaskSubpairOverlaps
        {
            //For now, we don't use anything tricky. Just traverse every child against the tree sequentially.
            //TODO: This sequentializes a whole lot of cache misses. You could probably get some benefit out of traversing all pairs 'simultaneously'- that is, 
            //using the fact that we have lots of independent queries to ensure the CPU always has something to do.
            Enumerator<TSubpairOverlaps> enumerator;
            enumerator.Pool = pool;
            for (int i = 0; i < count; ++i)
            {
                ref var pair = ref pairs[i];
                ref var mesh = ref Unsafe.AsRef<Mesh>(pair.Container);
                var scaledMin = mesh.inverseScale * pair.Min;
                var scaledMax = mesh.inverseScale * pair.Max;
                enumerator.Overlaps = Unsafe.AsPointer(ref overlaps.GetOverlapsForPair(i));
                Tree.GetOverlaps(scaledMin, scaledMax, ref enumerator);
            }
        }

        unsafe struct SweepLeafTester<TOverlaps> : ISweepLeafTester where TOverlaps : ICollisionTaskSubpairOverlaps
        {
            public BufferPool Pool;
            public void* Overlaps;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void TestLeaf(int leafIndex, ref float maximumT)
            {
                Unsafe.AsRef<TOverlaps>(Overlaps).Allocate(Pool) = leafIndex;
            }
        }
        public unsafe void FindLocalOverlaps<TOverlaps>(in Vector3 min, in Vector3 max, in Vector3 sweep, float maximumT, BufferPool pool, Shapes shapes, void* overlaps)
            where TOverlaps : ICollisionTaskSubpairOverlaps
        {
            var scaledMin = min * inverseScale;
            var scaledMax = max * inverseScale;
            var scaledSweep = sweep * inverseScale;
            SweepLeafTester<TOverlaps> enumerator;
            enumerator.Pool = pool;
            enumerator.Overlaps = overlaps;
            Tree.Sweep(scaledMin, scaledMax, scaledSweep, maximumT, ref enumerator);
        }


        public void Dispose(BufferPool bufferPool)
        {
            bufferPool.Return(ref Triangles);
            Tree.Dispose(bufferPool);
        }


        /// <summary>
        /// Type id of mesh shapes.
        /// </summary>
        public const int Id = 8;
        public int TypeId => Id;

    }
}
