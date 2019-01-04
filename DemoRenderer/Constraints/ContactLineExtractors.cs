﻿using BepuPhysics.CollisionDetection;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using DemoRenderer.Constraints;
using BepuUtilities.Collections;

namespace BepuPhysics.Constraints.Contact
{  
    struct Contact1OneBodyLineExtractor : IConstraintLineExtractor<Contact1OneBodyPrestepData>
    {
        public int LinesPerConstraint => 2;

        public unsafe void ExtractLines(ref Contact1OneBodyPrestepData prestepBundle, int setIndex, int* bodyIndices,
            Bodies bodies, ref Vector3 tint, ref QuickList<LineInstance> lines)
        {
            var poseA = bodies.Sets[setIndex].Poses[bodyIndices[0]];
            ContactLines.Add(poseA, ref prestepBundle.OffsetA0, ref prestepBundle.Normal, ref prestepBundle.PenetrationDepth0, tint, ref lines);
        }
    }
    struct Contact2OneBodyLineExtractor : IConstraintLineExtractor<Contact2OneBodyPrestepData>
    {
        public int LinesPerConstraint => 4;

        public unsafe void ExtractLines(ref Contact2OneBodyPrestepData prestepBundle, int setIndex, int* bodyIndices,
            Bodies bodies, ref Vector3 tint, ref QuickList<LineInstance> lines)
        {
            var poseA = bodies.Sets[setIndex].Poses[bodyIndices[0]];
            ContactLines.Add(poseA, ref prestepBundle.OffsetA0, ref prestepBundle.Normal, ref prestepBundle.PenetrationDepth0, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.OffsetA1, ref prestepBundle.Normal, ref prestepBundle.PenetrationDepth1, tint, ref lines);
        }
    }
    struct Contact3OneBodyLineExtractor : IConstraintLineExtractor<Contact3OneBodyPrestepData>
    {
        public int LinesPerConstraint => 6;

        public unsafe void ExtractLines(ref Contact3OneBodyPrestepData prestepBundle, int setIndex, int* bodyIndices,
            Bodies bodies, ref Vector3 tint, ref QuickList<LineInstance> lines)
        {
            var poseA = bodies.Sets[setIndex].Poses[bodyIndices[0]];
            ContactLines.Add(poseA, ref prestepBundle.OffsetA0, ref prestepBundle.Normal, ref prestepBundle.PenetrationDepth0, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.OffsetA1, ref prestepBundle.Normal, ref prestepBundle.PenetrationDepth1, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.OffsetA2, ref prestepBundle.Normal, ref prestepBundle.PenetrationDepth2, tint, ref lines);
        }
    }
    struct Contact4OneBodyLineExtractor : IConstraintLineExtractor<Contact4OneBodyPrestepData>
    {
        public int LinesPerConstraint => 8;

        public unsafe void ExtractLines(ref Contact4OneBodyPrestepData prestepBundle, int setIndex, int* bodyIndices,
            Bodies bodies, ref Vector3 tint, ref QuickList<LineInstance> lines)
        {
            var poseA = bodies.Sets[setIndex].Poses[bodyIndices[0]];
            ContactLines.Add(poseA, ref prestepBundle.OffsetA0, ref prestepBundle.Normal, ref prestepBundle.PenetrationDepth0, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.OffsetA1, ref prestepBundle.Normal, ref prestepBundle.PenetrationDepth1, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.OffsetA2, ref prestepBundle.Normal, ref prestepBundle.PenetrationDepth2, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.OffsetA3, ref prestepBundle.Normal, ref prestepBundle.PenetrationDepth3, tint, ref lines);
        }
    }
    struct Contact1LineExtractor : IConstraintLineExtractor<Contact1PrestepData>
    {
        public int LinesPerConstraint => 2;

        public unsafe void ExtractLines(ref Contact1PrestepData prestepBundle, int setIndex, int* bodyIndices,
            Bodies bodies, ref Vector3 tint, ref QuickList<LineInstance> lines)
        {
            var poseA = bodies.Sets[setIndex].Poses[bodyIndices[0]];
            ContactLines.Add(poseA, ref prestepBundle.OffsetA0, ref prestepBundle.Normal, ref prestepBundle.PenetrationDepth0, tint, ref lines);
        }
    }
    struct Contact2LineExtractor : IConstraintLineExtractor<Contact2PrestepData>
    {
        public int LinesPerConstraint => 4;

        public unsafe void ExtractLines(ref Contact2PrestepData prestepBundle, int setIndex, int* bodyIndices,
            Bodies bodies, ref Vector3 tint, ref QuickList<LineInstance> lines)
        {
            var poseA = bodies.Sets[setIndex].Poses[bodyIndices[0]];
            ContactLines.Add(poseA, ref prestepBundle.OffsetA0, ref prestepBundle.Normal, ref prestepBundle.PenetrationDepth0, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.OffsetA1, ref prestepBundle.Normal, ref prestepBundle.PenetrationDepth1, tint, ref lines);
        }
    }
    struct Contact3LineExtractor : IConstraintLineExtractor<Contact3PrestepData>
    {
        public int LinesPerConstraint => 6;

        public unsafe void ExtractLines(ref Contact3PrestepData prestepBundle, int setIndex, int* bodyIndices,
            Bodies bodies, ref Vector3 tint, ref QuickList<LineInstance> lines)
        {
            var poseA = bodies.Sets[setIndex].Poses[bodyIndices[0]];
            ContactLines.Add(poseA, ref prestepBundle.OffsetA0, ref prestepBundle.Normal, ref prestepBundle.PenetrationDepth0, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.OffsetA1, ref prestepBundle.Normal, ref prestepBundle.PenetrationDepth1, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.OffsetA2, ref prestepBundle.Normal, ref prestepBundle.PenetrationDepth2, tint, ref lines);
        }
    }
    struct Contact4LineExtractor : IConstraintLineExtractor<Contact4PrestepData>
    {
        public int LinesPerConstraint => 8;

        public unsafe void ExtractLines(ref Contact4PrestepData prestepBundle, int setIndex, int* bodyIndices,
            Bodies bodies, ref Vector3 tint, ref QuickList<LineInstance> lines)
        {
            var poseA = bodies.Sets[setIndex].Poses[bodyIndices[0]];
            ContactLines.Add(poseA, ref prestepBundle.OffsetA0, ref prestepBundle.Normal, ref prestepBundle.PenetrationDepth0, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.OffsetA1, ref prestepBundle.Normal, ref prestepBundle.PenetrationDepth1, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.OffsetA2, ref prestepBundle.Normal, ref prestepBundle.PenetrationDepth2, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.OffsetA3, ref prestepBundle.Normal, ref prestepBundle.PenetrationDepth3, tint, ref lines);
        }
    }
    struct Contact2NonconvexOneBodyLineExtractor : IConstraintLineExtractor<Contact2NonconvexOneBodyPrestepData>
    {
        public int LinesPerConstraint => 4;

        public unsafe void ExtractLines(ref Contact2NonconvexOneBodyPrestepData prestepBundle, int setIndex, int* bodyIndices,
            Bodies bodies, ref Vector3 tint, ref QuickList<LineInstance> lines)
        {
            var poseA = bodies.Sets[setIndex].Poses[bodyIndices[0]];
            ContactLines.Add(poseA, ref prestepBundle.Contact0.Offset, ref prestepBundle.Contact0.Normal, ref prestepBundle.Contact0.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact1.Offset, ref prestepBundle.Contact1.Normal, ref prestepBundle.Contact1.Depth, tint, ref lines);
        }
    }
    struct Contact3NonconvexOneBodyLineExtractor : IConstraintLineExtractor<Contact3NonconvexOneBodyPrestepData>
    {
        public int LinesPerConstraint => 6;

        public unsafe void ExtractLines(ref Contact3NonconvexOneBodyPrestepData prestepBundle, int setIndex, int* bodyIndices,
            Bodies bodies, ref Vector3 tint, ref QuickList<LineInstance> lines)
        {
            var poseA = bodies.Sets[setIndex].Poses[bodyIndices[0]];
            ContactLines.Add(poseA, ref prestepBundle.Contact0.Offset, ref prestepBundle.Contact0.Normal, ref prestepBundle.Contact0.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact1.Offset, ref prestepBundle.Contact1.Normal, ref prestepBundle.Contact1.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact2.Offset, ref prestepBundle.Contact2.Normal, ref prestepBundle.Contact2.Depth, tint, ref lines);
        }
    }
    struct Contact4NonconvexOneBodyLineExtractor : IConstraintLineExtractor<Contact4NonconvexOneBodyPrestepData>
    {
        public int LinesPerConstraint => 8;

        public unsafe void ExtractLines(ref Contact4NonconvexOneBodyPrestepData prestepBundle, int setIndex, int* bodyIndices,
            Bodies bodies, ref Vector3 tint, ref QuickList<LineInstance> lines)
        {
            var poseA = bodies.Sets[setIndex].Poses[bodyIndices[0]];
            ContactLines.Add(poseA, ref prestepBundle.Contact0.Offset, ref prestepBundle.Contact0.Normal, ref prestepBundle.Contact0.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact1.Offset, ref prestepBundle.Contact1.Normal, ref prestepBundle.Contact1.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact2.Offset, ref prestepBundle.Contact2.Normal, ref prestepBundle.Contact2.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact3.Offset, ref prestepBundle.Contact3.Normal, ref prestepBundle.Contact3.Depth, tint, ref lines);
        }
    }
    struct Contact5NonconvexOneBodyLineExtractor : IConstraintLineExtractor<Contact5NonconvexOneBodyPrestepData>
    {
        public int LinesPerConstraint => 10;

        public unsafe void ExtractLines(ref Contact5NonconvexOneBodyPrestepData prestepBundle, int setIndex, int* bodyIndices,
            Bodies bodies, ref Vector3 tint, ref QuickList<LineInstance> lines)
        {
            var poseA = bodies.Sets[setIndex].Poses[bodyIndices[0]];
            ContactLines.Add(poseA, ref prestepBundle.Contact0.Offset, ref prestepBundle.Contact0.Normal, ref prestepBundle.Contact0.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact1.Offset, ref prestepBundle.Contact1.Normal, ref prestepBundle.Contact1.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact2.Offset, ref prestepBundle.Contact2.Normal, ref prestepBundle.Contact2.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact3.Offset, ref prestepBundle.Contact3.Normal, ref prestepBundle.Contact3.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact4.Offset, ref prestepBundle.Contact4.Normal, ref prestepBundle.Contact4.Depth, tint, ref lines);
        }
    }
    struct Contact6NonconvexOneBodyLineExtractor : IConstraintLineExtractor<Contact6NonconvexOneBodyPrestepData>
    {
        public int LinesPerConstraint => 12;

        public unsafe void ExtractLines(ref Contact6NonconvexOneBodyPrestepData prestepBundle, int setIndex, int* bodyIndices,
            Bodies bodies, ref Vector3 tint, ref QuickList<LineInstance> lines)
        {
            var poseA = bodies.Sets[setIndex].Poses[bodyIndices[0]];
            ContactLines.Add(poseA, ref prestepBundle.Contact0.Offset, ref prestepBundle.Contact0.Normal, ref prestepBundle.Contact0.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact1.Offset, ref prestepBundle.Contact1.Normal, ref prestepBundle.Contact1.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact2.Offset, ref prestepBundle.Contact2.Normal, ref prestepBundle.Contact2.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact3.Offset, ref prestepBundle.Contact3.Normal, ref prestepBundle.Contact3.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact4.Offset, ref prestepBundle.Contact4.Normal, ref prestepBundle.Contact4.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact5.Offset, ref prestepBundle.Contact5.Normal, ref prestepBundle.Contact5.Depth, tint, ref lines);
        }
    }
    struct Contact7NonconvexOneBodyLineExtractor : IConstraintLineExtractor<Contact7NonconvexOneBodyPrestepData>
    {
        public int LinesPerConstraint => 14;

        public unsafe void ExtractLines(ref Contact7NonconvexOneBodyPrestepData prestepBundle, int setIndex, int* bodyIndices,
            Bodies bodies, ref Vector3 tint, ref QuickList<LineInstance> lines)
        {
            var poseA = bodies.Sets[setIndex].Poses[bodyIndices[0]];
            ContactLines.Add(poseA, ref prestepBundle.Contact0.Offset, ref prestepBundle.Contact0.Normal, ref prestepBundle.Contact0.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact1.Offset, ref prestepBundle.Contact1.Normal, ref prestepBundle.Contact1.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact2.Offset, ref prestepBundle.Contact2.Normal, ref prestepBundle.Contact2.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact3.Offset, ref prestepBundle.Contact3.Normal, ref prestepBundle.Contact3.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact4.Offset, ref prestepBundle.Contact4.Normal, ref prestepBundle.Contact4.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact5.Offset, ref prestepBundle.Contact5.Normal, ref prestepBundle.Contact5.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact6.Offset, ref prestepBundle.Contact6.Normal, ref prestepBundle.Contact6.Depth, tint, ref lines);
        }
    }
    struct Contact8NonconvexOneBodyLineExtractor : IConstraintLineExtractor<Contact8NonconvexOneBodyPrestepData>
    {
        public int LinesPerConstraint => 16;

        public unsafe void ExtractLines(ref Contact8NonconvexOneBodyPrestepData prestepBundle, int setIndex, int* bodyIndices,
            Bodies bodies, ref Vector3 tint, ref QuickList<LineInstance> lines)
        {
            var poseA = bodies.Sets[setIndex].Poses[bodyIndices[0]];
            ContactLines.Add(poseA, ref prestepBundle.Contact0.Offset, ref prestepBundle.Contact0.Normal, ref prestepBundle.Contact0.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact1.Offset, ref prestepBundle.Contact1.Normal, ref prestepBundle.Contact1.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact2.Offset, ref prestepBundle.Contact2.Normal, ref prestepBundle.Contact2.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact3.Offset, ref prestepBundle.Contact3.Normal, ref prestepBundle.Contact3.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact4.Offset, ref prestepBundle.Contact4.Normal, ref prestepBundle.Contact4.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact5.Offset, ref prestepBundle.Contact5.Normal, ref prestepBundle.Contact5.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact6.Offset, ref prestepBundle.Contact6.Normal, ref prestepBundle.Contact6.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact7.Offset, ref prestepBundle.Contact7.Normal, ref prestepBundle.Contact7.Depth, tint, ref lines);
        }
    }
    struct Contact2NonconvexLineExtractor : IConstraintLineExtractor<Contact2NonconvexPrestepData>
    {
        public int LinesPerConstraint => 4;

        public unsafe void ExtractLines(ref Contact2NonconvexPrestepData prestepBundle, int setIndex, int* bodyIndices,
            Bodies bodies, ref Vector3 tint, ref QuickList<LineInstance> lines)
        {
            var poseA = bodies.Sets[setIndex].Poses[bodyIndices[0]];
            ContactLines.Add(poseA, ref prestepBundle.Contact0.Offset, ref prestepBundle.Contact0.Normal, ref prestepBundle.Contact0.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact1.Offset, ref prestepBundle.Contact1.Normal, ref prestepBundle.Contact1.Depth, tint, ref lines);
        }
    }
    struct Contact3NonconvexLineExtractor : IConstraintLineExtractor<Contact3NonconvexPrestepData>
    {
        public int LinesPerConstraint => 6;

        public unsafe void ExtractLines(ref Contact3NonconvexPrestepData prestepBundle, int setIndex, int* bodyIndices,
            Bodies bodies, ref Vector3 tint, ref QuickList<LineInstance> lines)
        {
            var poseA = bodies.Sets[setIndex].Poses[bodyIndices[0]];
            ContactLines.Add(poseA, ref prestepBundle.Contact0.Offset, ref prestepBundle.Contact0.Normal, ref prestepBundle.Contact0.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact1.Offset, ref prestepBundle.Contact1.Normal, ref prestepBundle.Contact1.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact2.Offset, ref prestepBundle.Contact2.Normal, ref prestepBundle.Contact2.Depth, tint, ref lines);
        }
    }
    struct Contact4NonconvexLineExtractor : IConstraintLineExtractor<Contact4NonconvexPrestepData>
    {
        public int LinesPerConstraint => 8;

        public unsafe void ExtractLines(ref Contact4NonconvexPrestepData prestepBundle, int setIndex, int* bodyIndices,
            Bodies bodies, ref Vector3 tint, ref QuickList<LineInstance> lines)
        {
            var poseA = bodies.Sets[setIndex].Poses[bodyIndices[0]];
            ContactLines.Add(poseA, ref prestepBundle.Contact0.Offset, ref prestepBundle.Contact0.Normal, ref prestepBundle.Contact0.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact1.Offset, ref prestepBundle.Contact1.Normal, ref prestepBundle.Contact1.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact2.Offset, ref prestepBundle.Contact2.Normal, ref prestepBundle.Contact2.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact3.Offset, ref prestepBundle.Contact3.Normal, ref prestepBundle.Contact3.Depth, tint, ref lines);
        }
    }
    struct Contact5NonconvexLineExtractor : IConstraintLineExtractor<Contact5NonconvexPrestepData>
    {
        public int LinesPerConstraint => 10;

        public unsafe void ExtractLines(ref Contact5NonconvexPrestepData prestepBundle, int setIndex, int* bodyIndices,
            Bodies bodies, ref Vector3 tint, ref QuickList<LineInstance> lines)
        {
            var poseA = bodies.Sets[setIndex].Poses[bodyIndices[0]];
            ContactLines.Add(poseA, ref prestepBundle.Contact0.Offset, ref prestepBundle.Contact0.Normal, ref prestepBundle.Contact0.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact1.Offset, ref prestepBundle.Contact1.Normal, ref prestepBundle.Contact1.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact2.Offset, ref prestepBundle.Contact2.Normal, ref prestepBundle.Contact2.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact3.Offset, ref prestepBundle.Contact3.Normal, ref prestepBundle.Contact3.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact4.Offset, ref prestepBundle.Contact4.Normal, ref prestepBundle.Contact4.Depth, tint, ref lines);
        }
    }
    struct Contact6NonconvexLineExtractor : IConstraintLineExtractor<Contact6NonconvexPrestepData>
    {
        public int LinesPerConstraint => 12;

        public unsafe void ExtractLines(ref Contact6NonconvexPrestepData prestepBundle, int setIndex, int* bodyIndices,
            Bodies bodies, ref Vector3 tint, ref QuickList<LineInstance> lines)
        {
            var poseA = bodies.Sets[setIndex].Poses[bodyIndices[0]];
            ContactLines.Add(poseA, ref prestepBundle.Contact0.Offset, ref prestepBundle.Contact0.Normal, ref prestepBundle.Contact0.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact1.Offset, ref prestepBundle.Contact1.Normal, ref prestepBundle.Contact1.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact2.Offset, ref prestepBundle.Contact2.Normal, ref prestepBundle.Contact2.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact3.Offset, ref prestepBundle.Contact3.Normal, ref prestepBundle.Contact3.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact4.Offset, ref prestepBundle.Contact4.Normal, ref prestepBundle.Contact4.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact5.Offset, ref prestepBundle.Contact5.Normal, ref prestepBundle.Contact5.Depth, tint, ref lines);
        }
    }
    struct Contact7NonconvexLineExtractor : IConstraintLineExtractor<Contact7NonconvexPrestepData>
    {
        public int LinesPerConstraint => 14;

        public unsafe void ExtractLines(ref Contact7NonconvexPrestepData prestepBundle, int setIndex, int* bodyIndices,
            Bodies bodies, ref Vector3 tint, ref QuickList<LineInstance> lines)
        {
            var poseA = bodies.Sets[setIndex].Poses[bodyIndices[0]];
            ContactLines.Add(poseA, ref prestepBundle.Contact0.Offset, ref prestepBundle.Contact0.Normal, ref prestepBundle.Contact0.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact1.Offset, ref prestepBundle.Contact1.Normal, ref prestepBundle.Contact1.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact2.Offset, ref prestepBundle.Contact2.Normal, ref prestepBundle.Contact2.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact3.Offset, ref prestepBundle.Contact3.Normal, ref prestepBundle.Contact3.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact4.Offset, ref prestepBundle.Contact4.Normal, ref prestepBundle.Contact4.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact5.Offset, ref prestepBundle.Contact5.Normal, ref prestepBundle.Contact5.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact6.Offset, ref prestepBundle.Contact6.Normal, ref prestepBundle.Contact6.Depth, tint, ref lines);
        }
    }
    struct Contact8NonconvexLineExtractor : IConstraintLineExtractor<Contact8NonconvexPrestepData>
    {
        public int LinesPerConstraint => 16;

        public unsafe void ExtractLines(ref Contact8NonconvexPrestepData prestepBundle, int setIndex, int* bodyIndices,
            Bodies bodies, ref Vector3 tint, ref QuickList<LineInstance> lines)
        {
            var poseA = bodies.Sets[setIndex].Poses[bodyIndices[0]];
            ContactLines.Add(poseA, ref prestepBundle.Contact0.Offset, ref prestepBundle.Contact0.Normal, ref prestepBundle.Contact0.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact1.Offset, ref prestepBundle.Contact1.Normal, ref prestepBundle.Contact1.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact2.Offset, ref prestepBundle.Contact2.Normal, ref prestepBundle.Contact2.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact3.Offset, ref prestepBundle.Contact3.Normal, ref prestepBundle.Contact3.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact4.Offset, ref prestepBundle.Contact4.Normal, ref prestepBundle.Contact4.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact5.Offset, ref prestepBundle.Contact5.Normal, ref prestepBundle.Contact5.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact6.Offset, ref prestepBundle.Contact6.Normal, ref prestepBundle.Contact6.Depth, tint, ref lines);
            ContactLines.Add(poseA, ref prestepBundle.Contact7.Offset, ref prestepBundle.Contact7.Normal, ref prestepBundle.Contact7.Depth, tint, ref lines);
        }
    }
}
