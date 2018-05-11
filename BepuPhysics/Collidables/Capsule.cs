﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using BepuUtilities.Memory;
using System.Diagnostics;
using Quaternion = BepuUtilities.Quaternion;
using BepuUtilities;

namespace BepuPhysics.Collidables
{

    /// <summary>
    /// Collision shape representing a sphere-expanded line segment.
    /// </summary>
    public struct Capsule : IConvexShape
    {
        /// <summary>
        /// Spherical expansion applied to the internal line segment.
        /// </summary>
        public float Radius;
        /// <summary>
        /// Half of the length of the internal line segment. Oriented along the local Y axis.
        /// </summary>
        public float HalfLength;

        /// <summary>
        /// Gets or sets the length of the capsule.
        /// </summary>
        public float Length { get { return HalfLength * 2; } set { HalfLength = value * 0.5f; } }

        public Capsule(float radius, float length)
        {
            Radius = radius;
            HalfLength = length * 0.5f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ComputeAngularExpansionData(out float maximumRadius, out float maximumAngularExpansion)
        {
            maximumRadius = HalfLength + Radius;
            //The minimum radius is capsules.Radius, so the maximum offset is simply the half length.
            maximumAngularExpansion = HalfLength;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ComputeBounds(in Quaternion orientation, out Vector3 min, out Vector3 max)
        {
            Quaternion.TransformUnitY(orientation, out var segmentOffset);
            max = Vector3.Abs(HalfLength * segmentOffset) + new Vector3(Radius);
            min = -max;
        }


        public bool RayTest(in RigidPose pose, in Vector3 origin, in Vector3 direction, out float t, out Vector3 normal)
        {
            //It's convenient to work in local space, so pull the ray into the capsule's local space.
            Matrix3x3.CreateFromQuaternion(pose.Orientation, out var orientation);
            var o = origin - pose.Position;
            Matrix3x3.TransformTranspose(o, ref orientation, out o);
            Matrix3x3.TransformTranspose(direction, ref orientation, out var d);

            //Normalize the direction. Sqrts aren't *that* bad, and it both simplifies things and helps avoid numerical problems.
            var inverseDLength = 1f / d.Length();
            d *= inverseDLength;

            //Move the origin up to the earliest possible impact time. This isn't necessary for math reasons, but it does help avoid some numerical problems.
            var tOffset = -Vector3.Dot(o, d) - (HalfLength + Radius);
            if (tOffset < 0)
                tOffset = 0;
            o += d * tOffset;
            var oh = new Vector3(o.X, 0, o.Z);
            var dh = new Vector3(d.X, 0, d.Z);
            var a = Vector3.Dot(dh, dh);
            var b = Vector3.Dot(oh, dh);
            var radiusSquared = Radius * Radius;
            var c = Vector3.Dot(oh, oh) - radiusSquared;
            if (b > 0 && c > 0)
            {
                //Ray is outside and pointing away, no hit.
                t = 0;
                normal = new Vector3();
                return false;
            }

            float sphereY;
            if (a > 1e-8f)
            {
                var discriminant = b * b - a * c;
                if (discriminant < 0)
                {
                    //The infinite cylinder isn't hit, so the capsule can't be hit.
                    t = 0;
                    normal = new Vector3();
                    return false;
                }
                t = (-b - (float)Math.Sqrt(discriminant)) / a;
                if (t < -tOffset)
                    t = -tOffset;
                var cylinderHitLocation = o + d * t;
                if (cylinderHitLocation.Y < -HalfLength)
                {
                    sphereY = -HalfLength;
                }
                else if (cylinderHitLocation.Y > HalfLength)
                {
                    sphereY = HalfLength;
                }
                else
                {
                    //The hit is on the cylindrical portion of the capsule.
                    normal = new Vector3(cylinderHitLocation.X, 0, cylinderHitLocation.Z) / Radius;
                    Matrix3x3.Transform(ref normal, ref orientation, out normal);
                    t = (t + tOffset) * inverseDLength;
                    return true;
                }
            }
            else
            {
                //The ray is parallel to the axis; the impact is on a spherical cap or nothing.
                sphereY = d.Y > 0 ? -HalfLength : HalfLength;
            }

            var os = o - new Vector3(0, sphereY, 0);
            var capB = Vector3.Dot(os, d);
            var capC = Vector3.Dot(os, os) - radiusSquared;

            if (capB > 0 && capC > 0)
            {
                //Ray is outside and pointing away, no hit.
                t = 0;
                normal = new Vector3();
                return false;
            }

            var capDiscriminant = capB * capB - capC;
            if (capDiscriminant < 0)
            {
                //Ray misses, no hit.
                t = 0;
                normal = new Vector3();
                return false;
            }
            t = -capB - (float)Math.Sqrt(capDiscriminant);
            if (t < -tOffset)
                t = -tOffset;
            normal = (os + d * t) / Radius;
            t = (t + tOffset) * inverseDLength;
            Matrix3x3.Transform(ref normal, ref orientation, out normal);
            return true;

        }

        public void ComputeInertia(float mass, out BodyInertia inertia)
        {
            inertia.InverseMass = 1f / mass;
            var r2 = Radius * Radius;
            var h2 = HalfLength * HalfLength;
            var cylinderVolume = 2 * HalfLength * r2 * MathHelper.Pi;
            var sphereVolume = (4f / 3f) * r2 * Radius * MathHelper.Pi;
            var inverseTotal = 1f / (cylinderVolume + sphereVolume);
            //Volume is in units of the capsule's whole volume.
            cylinderVolume *= inverseTotal;
            sphereVolume *= inverseTotal;
            inertia.InverseInertiaTensor.XX = inertia.InverseMass / (
                cylinderVolume * ((3f / 12f) * r2 + (4f / 12f) * h2) +
                sphereVolume * ((2f / 5f) * r2 + (6f / 8f) * Radius * HalfLength + h2));
            inertia.InverseInertiaTensor.YX = 0;
            inertia.InverseInertiaTensor.YY = inertia.InverseMass / (cylinderVolume * (1f / 2f) * r2 + sphereVolume * (2f / 5f) * r2);
            inertia.InverseInertiaTensor.ZX = 0;
            inertia.InverseInertiaTensor.ZY = 0;
            inertia.InverseInertiaTensor.ZZ = inertia.InverseInertiaTensor.XX;
        }

        public ShapeBatch CreateShapeBatch(BufferPool pool, int initialCapacity, Shapes shapeBatches)
        {
            return new ConvexShapeBatch<Capsule, CapsuleWide>(pool, initialCapacity);
        }



        /// <summary>
        /// Type id of capsule shapes.
        /// </summary>
        public const int Id = 1;
        public int TypeId { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return Id; } }
    }

    public struct CapsuleWide : IShapeWide<Capsule>
    {
        public Vector<float> Radius;
        public Vector<float> HalfLength;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Broadcast(ref Capsule shape)
        {
            Radius = new Vector<float>(shape.Radius);
            HalfLength = new Vector<float>(shape.HalfLength);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Gather(ref Capsule source)
        {
            Unsafe.As<Vector<float>, float>(ref Radius) = source.Radius;
            Unsafe.As<Vector<float>, float>(ref HalfLength) = source.HalfLength;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetBounds(ref QuaternionWide orientations, out Vector<float> maximumRadius, out Vector<float> maximumAngularExpansion, out Vector3Wide min, out Vector3Wide max)
        {
            QuaternionWide.TransformUnitY(ref orientations, out var segmentOffset);
            Vector3Wide.Scale(ref segmentOffset, ref HalfLength, out segmentOffset);
            Vector3Wide.Abs(ref segmentOffset, out segmentOffset);

            //The half length extends symmetrically along positive local Y and negative local Y.
            Vector3Wide.Add(ref segmentOffset, ref Radius, out max);
            Vector3Wide.Negate(ref max, out min);

            maximumRadius = HalfLength + Radius;
            //The minimum radius is capsules.Radius, so the maximum offset is simply the half length.
            maximumAngularExpansion = HalfLength;
        }

        public int MinimumWideRayCount
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return 2;
            }
        }

        public void RayTest(ref RigidPoses pose, ref RayWide ray, out Vector<int> intersected, out Vector<float> t, out Vector3Wide normal)
        {
            //It's convenient to work in local space, so pull the ray into the capsule's local space.
            Matrix3x3Wide.CreateFromQuaternion(ref pose.Orientation, out var orientation);
            Vector3Wide.Subtract(ref ray.Origin, ref pose.Position, out var oWorld);
            Matrix3x3Wide.TransformByTransposedWithoutOverlap(ref oWorld, ref orientation, out var o);
            Matrix3x3Wide.TransformByTransposedWithoutOverlap(ref ray.Direction, ref orientation, out var d);

            //Normalize the direction. Sqrts aren't *that* bad, and it both simplifies things and helps avoid numerical problems.
            Vector3Wide.Length(ref d, out var dLength);
            var inverseDLength = Vector<float>.One / dLength;
            Vector3Wide.Scale(ref d, ref inverseDLength, out d);

            //Move the origin up to the earliest possible impact time. This isn't necessary for math reasons, but it does help avoid some numerical problems.
            Vector3Wide.Dot(ref o, ref d, out var od);
            var tOffset = Vector.Max(-od - (HalfLength + Radius), Vector<float>.Zero);
            Vector3Wide.Scale(ref d, ref tOffset, out var oOffset);
            Vector3Wide.Add(ref o, ref oOffset, out o);
            var a = d.X * d.X + d.Z * d.Z;
            var b = o.X * d.X + o.Z * d.Z;
            var radiusSquared = Radius * Radius;
            var c = (o.X * o.X + o.Z * o.Z) - radiusSquared;

            var rayIsntParallel = Vector.GreaterThan(a, new Vector<float>(1e-8f));
            var discriminant = b * b - a * c;
            var cylinderIntersected = Vector.BitwiseAnd(
                Vector.BitwiseOr(
                    Vector.LessThanOrEqual(b, Vector<float>.Zero),
                    Vector.LessThanOrEqual(c, Vector<float>.Zero)),
                Vector.GreaterThanOrEqual(discriminant, Vector<float>.Zero));
            var cylinderT = Vector.Max(-tOffset, (-b - Vector.SquareRoot(discriminant)) / a);
            Vector3Wide.Scale(ref d, ref cylinderT, out oOffset);
            Vector3Wide.Add(ref o, ref oOffset, out var cylinderHitLocation);
            var inverseRadius = Vector<float>.One / Radius;
            var cylinderNormalX = cylinderHitLocation.X * inverseRadius;
            var cylinderNormalZ = cylinderHitLocation.Z * inverseRadius;
            var useCylinder = Vector.BitwiseAnd(Vector.GreaterThanOrEqual(cylinderHitLocation.Y, -HalfLength), Vector.LessThanOrEqual(cylinderHitLocation.Y, HalfLength));

            //Intersect the spherical cap for any lane which ended up not using the cylinder.
            Vector<float> sphereY = Vector.ConditionalSelect(
                Vector.BitwiseOr(
                    Vector.BitwiseAnd(Vector.GreaterThan(cylinderHitLocation.Y, HalfLength), rayIsntParallel),
                    Vector.AndNot(Vector.LessThanOrEqual(d.Y, Vector<float>.Zero), rayIsntParallel)), HalfLength, -HalfLength);

            o.Y -= sphereY;
            Vector3Wide.Dot(ref o, ref d, out var capB);
            Vector3Wide.Dot(ref o, ref o, out var capC);
            capC -= radiusSquared;

            var capDiscriminant = capB * capB - capC;
            var capIntersected = Vector.BitwiseAnd(
                Vector.BitwiseOr(
                    Vector.LessThanOrEqual(capB, Vector<float>.Zero),
                    Vector.LessThanOrEqual(capC, Vector<float>.Zero)),
                Vector.GreaterThanOrEqual(capDiscriminant, Vector<float>.Zero));

            var capT = Vector.Max(-tOffset, -capB - Vector.SquareRoot(capDiscriminant));
            Vector3Wide.Scale(ref d, ref capT, out oOffset);
            Vector3Wide.Add(ref o, ref oOffset, out var capHitLocation);
            Vector3Wide.Scale(ref capHitLocation, ref inverseRadius, out var capNormal);

            normal.X = Vector.ConditionalSelect(useCylinder, cylinderNormalX, capNormal.X);
            normal.Y = Vector.ConditionalSelect(useCylinder, Vector<float>.Zero, capNormal.Y);
            normal.Z = Vector.ConditionalSelect(useCylinder, cylinderNormalZ, capNormal.Z);
            t = (Vector.ConditionalSelect(useCylinder, cylinderT, capT) + tOffset) * inverseDLength;
            intersected = Vector.ConditionalSelect(useCylinder, cylinderIntersected, capIntersected);
            Matrix3x3Wide.Transform(ref normal, ref orientation, out normal);
        }
    }
}
