using System;
using Leap;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DualLeap
{
    public static class LeapDisassembly
    {
        public static readonly int MaxSize = 60000;

        public static readonly int Size = 2050;

        public static byte[] Disassembly(List<Hand> hands)
        { 
            var bytes = new List<byte>();

            Parallel.ForEach(hands, (x) =>
            {
               var data = Disassembly(x);
               lock (bytes)
               {
                   bytes.AddRange(data);
               }
            });

            return bytes.ToArray();
        }
        // Handクラスのオブジェクトを値型の値に分解
        // そのあとUDPで送るためにバイト配列に変換する
        public static byte[] Disassembly(Hand hand)
        {
            var bytes = new List<byte>();

            bytes.AddRange(BitConverter.GetBytes(hand.FrameId));

            bytes.AddRange(BitConverter.GetBytes(hand.Id));

            bytes.AddRange(BitConverter.GetBytes(hand.Confidence));

            bytes.AddRange(BitConverter.GetBytes(hand.GrabStrength));

            bytes.AddRange(BitConverter.GetBytes(hand.GrabAngle));

            bytes.AddRange(BitConverter.GetBytes(hand.PinchStrength));

            bytes.AddRange(BitConverter.GetBytes(hand.PinchDistance));

            bytes.AddRange(BitConverter.GetBytes(hand.PalmWidth));

            bytes.AddRange(BitConverter.GetBytes(hand.IsLeft));

            bytes.AddRange(BitConverter.GetBytes(hand.TimeVisible));

            ArmConvertByteAddRange(hand.Arm, bytes);

            FingersConvertByteAddRange(hand.Fingers, bytes);

            // Vectorを3つのfloat型の変数に分解して変換する
            VectorConvertByteAddRange(hand.PalmPosition, bytes);

            VectorConvertByteAddRange(hand.StabilizedPalmPosition, bytes);

            VectorConvertByteAddRange(hand.PalmVelocity, bytes);

            VectorConvertByteAddRange(hand.PalmNormal, bytes);

            VectorConvertByteAddRange(hand.Direction, bytes);

            VectorConvertByteAddRange(hand.WristPosition, bytes);

            return bytes.ToArray();
        }
        // Vectorを3つのfloat型の変数に分解して変換するメソッドの実装
        static void VectorConvertByteAddRange(Vector value, List<byte> list)
        {
            list.AddRange(BitConverter.GetBytes(value.x));

            list.AddRange(BitConverter.GetBytes(value.y));

            list.AddRange(BitConverter.GetBytes(value.z));
        }
        // LeapQuaternionを4つのfloat型の変数に分解して変換するメソッドの実装
        static void LeapQuaternionConvertByteAddRange(LeapQuaternion value, List<byte> list)
        {
            list.AddRange(BitConverter.GetBytes(value.x));

            list.AddRange(BitConverter.GetBytes(value.y));

            list.AddRange(BitConverter.GetBytes(value.z));

            list.AddRange(BitConverter.GetBytes(value.w));
        }
        // Armをいくつかの変数に分解して変換するメソッドの実装
        static void ArmConvertByteAddRange(Arm arm, List<byte> list)
        {
            VectorConvertByteAddRange(arm.ElbowPosition, list);

            VectorConvertByteAddRange(arm.WristPosition, list);

            VectorConvertByteAddRange(arm.Center, list);

            VectorConvertByteAddRange(arm.Direction, list);

            list.AddRange(BitConverter.GetBytes(arm.Length));

            list.AddRange(BitConverter.GetBytes(arm.Width));

            LeapQuaternionConvertByteAddRange(arm.Rotation, list);
        }
        // List<Finger>の中身をFingerConvertByteAddRangeに渡す
        static void FingersConvertByteAddRange(List<Finger> fingers, List<byte> list)
        {
            foreach (var finger in fingers)
            {
                FingerConvertByteAddRange(finger, list);
            }
        }
        // Fingerをいくつかの変数に分解して変換するメソッドの実装
        static void FingerConvertByteAddRange(Finger finger, List<byte> list)
        {
            list.AddRange(BitConverter.GetBytes(finger.Id));

            list.AddRange(BitConverter.GetBytes(finger.TimeVisible));

            VectorConvertByteAddRange(finger.TipPosition, list);

            VectorConvertByteAddRange(finger.TipVelocity, list);

            VectorConvertByteAddRange(finger.Direction, list);

            VectorConvertByteAddRange(finger.StabilizedTipPosition, list);

            list.AddRange(BitConverter.GetBytes(finger.Width));

            list.AddRange(BitConverter.GetBytes(finger.Length));

            list.AddRange(BitConverter.GetBytes(finger.IsExtended));

            list.AddRange(BitConverter.GetBytes((int)finger.Type));

            BoneConvertByteAddRange(finger.Bone(Bone.BoneType.TYPE_METACARPAL), list);

            BoneConvertByteAddRange(finger.Bone(Bone.BoneType.TYPE_PROXIMAL), list);

            BoneConvertByteAddRange(finger.Bone(Bone.BoneType.TYPE_INTERMEDIATE), list);

            BoneConvertByteAddRange(finger.Bone(Bone.BoneType.TYPE_DISTAL), list);
        }
        // Boneをいくつかの変数に分解して変換するメソッドの実装
        static void BoneConvertByteAddRange(Bone bone, List<byte> list)
        {
            VectorConvertByteAddRange(bone.PrevJoint, list);

            VectorConvertByteAddRange(bone.NextJoint, list);

            VectorConvertByteAddRange(bone.Center, list);

            VectorConvertByteAddRange(bone.Direction, list);

            list.AddRange(BitConverter.GetBytes(bone.Length));

            list.AddRange(BitConverter.GetBytes(bone.Width));

            list.AddRange(BitConverter.GetBytes((int)bone.Type));

            LeapQuaternionConvertByteAddRange(bone.Rotation, list);
        }
    }
}