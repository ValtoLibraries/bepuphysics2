﻿using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BepuPhysics.CollisionDetection
{
    /// <summary>
    /// Information about a single contact in a nonconvex collidable pair.
    /// Nonconvex pairs can have different surface bases at each contact point, since the contact surface is not guaranteed to be a plane.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 32)]
    public struct NonconvexContact
    {
        /// <summary>
        /// Offset from the position of collidable A to the contact position. 
        /// </summary>
        [FieldOffset(0)]
        public Vector3 Offset;
        /// <summary>
        /// Penetration depth between the two collidables at this contact. Negative values represent separation.
        /// </summary>
        [FieldOffset(12)]
        public float Depth;
        /// <summary>
        /// Surface basis of the contact. If transformed into a rotation matrix, X and Z represent tangent directions and Y represents the contact normal.
        /// </summary>
        [FieldOffset(16)]
        public Vector3 Normal;
        /// <summary>
        /// Id of the features involved in the collision that generated this contact. If a contact has the same feature id as in a previous frame, it is an indication that the
        /// same parts of the shape contributed to its creation. This is useful for carrying information from frame to frame.
        /// </summary>
        [FieldOffset(28)]
        public int FeatureId;
    }
    /// <summary>
    /// Information about a single contact in a convex collidable pair. Convex collidable pairs share one surface basis across the manifold, since the contact surface is guaranteed to be a plane.
    /// </summary>    
    [StructLayout(LayoutKind.Explicit, Size = 20)]
    public struct ConvexContact
    {
        /// <summary>
        /// Offset from the position of collidable A to the contact position. 
        /// </summary>
        [FieldOffset(0)]
        public Vector3 Offset;
        /// <summary>
        /// Penetration depth between the two collidables at this contact. Negative values represent separation.
        /// </summary>
        [FieldOffset(12)]
        public float Depth;
        /// <summary>
        /// Id of the features involved in the collision that generated this contact. If a contact has the same feature id as in a previous frame, it is an indication that the
        /// same parts of the shape contributed to its creation. This is useful for carrying information from frame to frame.
        /// </summary>
        [FieldOffset(16)]
        public int FeatureId;
    }

    public interface IContactManifold
    {
        /// <summary>
        /// Gets the number of contacts in the manifold.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets whether the contact manifold was created by a pair of convex objects or not. True if convex, false if nonconvex.
        /// </summary>
        bool Convex { get; }

        /// <summary>
        /// Retrieves the feature id associated with a requested contact.
        /// </summary>
        /// <param name="contactIndex">Index of the contact to grab the feature id of.</param>
        /// <returns>Feature id of the requested contact.</returns>
        int GetFeatureId(int contactIndex);

        /// <summary>
        /// Retrieves a copy of a contact's data.
        /// </summary>
        /// <param name="contactIndex">Index of the contact to copy data from.</param>
        /// <param name="offset">Offset from the first collidable's position to the contact position.</param>
        /// <param name="normal">Normal of the contact surface at the requested contact.</param>
        /// <param name="depth">Penetration depth at the requested contact.</param>
        /// <param name="featureId">Feature id of the requested contact.
        /// Feature ids represent which parts of the collidables formed the contact and can be used to track unique contacts across frames.</param>
        void GetContact(int contactIndex, out Vector3 offset, out Vector3 normal, out float depth, out int featureId);

    }

    //TODO: We could use specialized storage types for things like continuations if L2 can't actually hold it all. Seems unlikely, but it's not that hard if required.

    /// <summary>
    /// Contains the data associated with a nonconvex contact manifold.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 272)]
    public unsafe struct NonconvexContactManifold : IContactManifold
    {
        /// <summary>
        /// Offset from collidable A to collidable B.
        /// </summary>
        [FieldOffset(0)]
        public Vector3 OffsetB;
        [FieldOffset(12)]
        public int Count;
        
        [FieldOffset(16)]
        public NonconvexContact Contact0;
        [FieldOffset(48)]
        public NonconvexContact Contact1;
        [FieldOffset(80)]
        public NonconvexContact Contact2;
        [FieldOffset(112)]
        public NonconvexContact Contact3;
        [FieldOffset(144)]
        public NonconvexContact Contact4;
        [FieldOffset(176)]
        public NonconvexContact Contact5;
        [FieldOffset(208)]
        public NonconvexContact Contact6;
        [FieldOffset(240)]
        public NonconvexContact Contact7;

        int IContactManifold.Count => Count;

        bool IContactManifold.Convex => false;

        [Conditional("DEBUG")]
        private void ValidateIndex(int contactIndex)
        {
            Debug.Assert(contactIndex >= 0 && contactIndex < Count, "Contact index must be within the contact count.");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetContact(int contactIndex, out Vector3 offset, out Vector3 normal, out float depth, out int featureId)
        {
            ValidateIndex(contactIndex);
            ref var contact = ref Unsafe.Add(ref Contact0, contactIndex);
            offset = contact.Offset;
            normal = contact.Normal;
            depth = contact.Depth;
            featureId = contact.FeatureId;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetFeatureId(int contactIndex)
        {
            ValidateIndex(contactIndex);
            return Unsafe.Add(ref Contact0, contactIndex).FeatureId;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FastRemoveAt(NonconvexContactManifold* manifold, int index)
        {
            --manifold->Count;
            if (index < manifold->Count)
            {
                var contacts = &manifold->Contact0;
                contacts[index] = contacts[manifold->Count];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add(NonconvexContactManifold* manifold, ref Vector3 normal, ref ConvexContact convexContact)
        {
            Debug.Assert(manifold->Count < 8);
            ref var targetContact = ref (&manifold->Contact0)[manifold->Count++];
            targetContact.Depth = convexContact.Depth;
            targetContact.Offset = convexContact.Offset;
            targetContact.Normal = normal;
            targetContact.FeatureId = convexContact.FeatureId;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref NonconvexContact Allocate(NonconvexContactManifold* manifold)
        {
            Debug.Assert(manifold->Count < 8);
            return ref (&manifold->Contact0)[manifold->Count++];
        }

    }

    /// <summary>
    /// Contains the data associated with a convex contact manifold.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 108)]
    public unsafe struct ConvexContactManifold : IContactManifold
    {
        /// <summary>
        /// Offset from collidable A to collidable B.
        /// </summary>
        [FieldOffset(0)]
        public Vector3 OffsetB;
        [FieldOffset(12)]
        public int Count;

        /// <summary>
        /// Surface normal shared by all contacts.
        /// </summary>
        [FieldOffset(16)]
        public Vector3 Normal;

        [FieldOffset(28)]
        public ConvexContact Contact0;
        [FieldOffset(48)]
        public ConvexContact Contact1;
        [FieldOffset(68)]
        public ConvexContact Contact2;
        [FieldOffset(88)]
        public ConvexContact Contact3;

        int IContactManifold.Count => Count;

        bool IContactManifold.Convex => true;

        [Conditional("DEBUG")]
        private void ValidateIndex(int contactIndex)
        {
            Debug.Assert(contactIndex >= 0 && contactIndex < Count, "Contact index must be within the contact count.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetFeatureId(int contactIndex)
        {
            ValidateIndex(contactIndex);
            return Unsafe.Add(ref Contact0, contactIndex).FeatureId;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetContact(int contactIndex, out Vector3 offset, out Vector3 normal, out float depth, out int featureId)
        {
            ValidateIndex(contactIndex);
            ref var contact = ref Unsafe.Add(ref Contact0, contactIndex);
            offset = contact.Offset;
            normal = Normal;
            depth = contact.Depth;
            featureId = contact.FeatureId;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FastRemoveAt(ConvexContactManifold* manifold, int index)
        {
            --manifold->Count;
            if (index < manifold->Count)
            {
                var contacts = &manifold->Contact0;
                contacts[index] = contacts[manifold->Count];
            }
        }
    }

}