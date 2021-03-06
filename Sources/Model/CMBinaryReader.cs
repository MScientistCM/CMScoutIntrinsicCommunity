using System;
using System.IO;

namespace CMScoutIntrinsic
{
    public class CMBinaryReader
    {
        private BinaryReader _br;
        private Boolean      _isCompressed;
        const int BUFFER_SIZE = 64 * 1024;
        byte[] Fbuffer = new byte[BUFFER_SIZE];
        bool FboBufEmpty = true;
        int FintPos = 0;
        int FintReadPos = 0;
        int FintBufPos = 0;

        public CMBinaryReader(BinaryReader br, Boolean isCompressed)
        {
            _br           = br;
            _isCompressed = isCompressed;
        }

        public long Seek(long offset, SeekOrigin origin)
        {
            FintPos = 0;
            FintReadPos = 0;
            FintBufPos = 0;
            FboBufEmpty = true;
            return _br.BaseStream.Seek(offset, origin);
        }

        private int Read(byte[] Buffer, int Count)
        {
            int intNewBufPos;
            int byByteCount;
            byte byMultByte;
            byte[] poTemp;

            if (!_isCompressed)
            {
                return _br.Read(Buffer, 0, Count);
            }
            else
            {
                if (Count > BUFFER_SIZE)
                {
                    return 0;
                }
            }

            if ((FintPos != FintReadPos + BUFFER_SIZE) || (FboBufEmpty))
            {
                FboBufEmpty = false;
                FintBufPos = 0;
                FintReadPos = (int)_br.BaseStream.Position;
                FintPos = FintReadPos + _br.Read(Fbuffer, 0, BUFFER_SIZE);
            }

            if (BUFFER_SIZE - FintBufPos < (Count * 2))
            {
                poTemp = new byte[BUFFER_SIZE - FintBufPos];
                Array.Copy(Fbuffer, FintBufPos, poTemp, 0, BUFFER_SIZE - FintBufPos);
                Array.Copy(poTemp, 0, Fbuffer, 0, BUFFER_SIZE - FintBufPos);
                FintReadPos = FintPos - (BUFFER_SIZE - FintBufPos);
                FintPos = FintReadPos + _br.Read(Fbuffer, BUFFER_SIZE - FintBufPos, BUFFER_SIZE - (BUFFER_SIZE - FintBufPos)) + (BUFFER_SIZE - FintBufPos);
                FintBufPos = 0;
            }

            intNewBufPos = 0;

            while (intNewBufPos < Count)
            {
                if (Fbuffer[FintBufPos] <= 128)
                {
                    //Byte(Pointer(Integer(@Buffer) + intNewBufPos)^) = Fbuffer[FintBufPos];
                    Buffer[intNewBufPos] = Fbuffer[FintBufPos];
                    intNewBufPos++;
                    FintBufPos++;
                }
                else
                {
                    byByteCount = Fbuffer[FintBufPos] - 128;
                    FintBufPos++;
                    byMultByte = Fbuffer[FintBufPos];
                    FintBufPos++;
                    if (byByteCount + intNewBufPos > Count)
                    {
                        FintBufPos -= 2;
                        Fbuffer[FintBufPos] = (byte)(byByteCount - (Count - intNewBufPos) + 128);
                        byByteCount = Count - intNewBufPos;
                    }
                    //FillChar(Byte(Pointer(Integer(@Buffer) + intNewBufPos)^), byByteCount, byMultByte);
                    for (int i = intNewBufPos; i < intNewBufPos + byByteCount; i++)
                        Buffer[i] = byMultByte;

                    intNewBufPos += byByteCount;
                }
                if (FintBufPos > BUFFER_SIZE)
                    FboBufEmpty = true;
            }

            return Count;
        }

        // -----
        public Byte[] ReadBytes(Int32 count) {
            if(!_isCompressed) {
                return _br.ReadBytes(count);
            }
            else {
                Byte[] bytes = new Byte[count];

                Read(bytes, count);

                return bytes;
            }
        }

        public Byte ReadByte() {
            if(!_isCompressed) {
                return _br.ReadByte();
            }
            else {
                Byte[] bytes = ReadBytes(1);

                return bytes[0];
            }
        }

        public SByte ReadSByte() {
            if(!_isCompressed) {
                return _br.ReadSByte();
            }
            else {
                Byte[] bytes = ReadBytes(1);

                return (SByte)(bytes[0]);
            }
        }

        public Int16 ReadInt16() {
            if(!_isCompressed) {
                return _br.ReadInt16();
            }
            else {
                Byte[] bytes = ReadBytes(2);

                return BitConverter.ToInt16(bytes, 0);
            }
        }

        public Int32 ReadInt32() {
            if(!_isCompressed) {
                return _br.ReadInt32();
            }
            else {
                Byte[] bytes = ReadBytes(4);

                return BitConverter.ToInt32(bytes, 0);
            }
        }

        public Double ReadDouble() {
            if(!_isCompressed) {
                return _br.ReadDouble();
            }
            else {
                Byte[] bytes = ReadBytes(8);

                return BitConverter.ToDouble(bytes, 0);
            }
        }
        // -----
    }
}