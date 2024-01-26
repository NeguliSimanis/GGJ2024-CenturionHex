using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ByteArray
{
    public int DataSize;

    bool endian = true;
    public int length = 0;
    public int position = 0;
    public byte[] arr = null;

    public ByteArray()
    {
        clear();
    }

    public void clear()
    {
        length = 0;
        position = 0;
        //arr = null;
        endian = true;//little endian
    }

    public void ensure( int additionalBytes )
    {
        if ((this.arr == null) || (this.arr.Length < this.position + additionalBytes))
        {
            //pad additionalBytes to 1024 border
            additionalBytes += (1024 - additionalBytes % 1024);
            //resize array
            var newSize = this.arr == null ? additionalBytes : this.arr.Length + additionalBytes;
            var tmp = new ByteArray();
            tmp.arr = this.arr;
            this.arr = new byte[newSize];
            if (tmp.arr != null)
            {
                //copy contents
                var pos = this.position;
                this.position = 0;
                tmp.readBytes(this, 0, pos);
            }
        }
    }

    public int bytesAvailable()
    {
        return this.length - this.position;
    }

    public void readBytes( ByteArray bytes, int pos, int len )
    {
        bytes.ensure(len);
        for (var i = position; i < len; i++)
            bytes.arr[pos + i] = arr[position + i];
        bytes.length += len;
        this.position += len;
    }

    public double readDouble()
    {
        double retval = BitConverter.ToDouble(arr, position);
        this.position += 8;
        return retval;
    }

    public float readFloat()
    {
        float retval = BitConverter.ToSingle(arr, position);
        this.position += 4;
        return retval;
    }

    public byte readByte()
    {
        return arr[position++];
    }

    public bool readBoolean()
    {
        return arr[position++] != 0;
    }

    public int readInt()
    {
        int retval = BitConverter.ToInt32(arr, position);
        this.position += 4;
        return retval;
    }

    public short readShort()
    {
        short retval = BitConverter.ToInt16(arr, position);
        this.position += 2;
        return retval;
    }

    public uint readUnsignedInt()
    {
        uint retval = BitConverter.ToUInt32(arr, position);
        this.position += 4;
        return retval;
    }

    public ushort readUnsignedShort()
    {
        ushort retval = BitConverter.ToUInt16(arr, position);
        this.position += 2;
        return retval;
    }

    public string readUTFBytes( int len )
    {
        string retval = System.Text.Encoding.UTF8.GetString(arr, position, len);
        this.position += len;
        return retval;
    }

    public string readUTF()
    {
        var len = this.readUnsignedShort();
        return this.readUTFBytes(len);
    }

    public void writeBytes( byte[] data)
    {
        ensure(data.Length);
        for( int k = 0; k < data.Length; k++ )
        {
            arr[position++] = data[k];
        }
        length += data.Length;
    }

    public void writeByte( byte val )
    {
        ensure(1);
        arr[position++] = val;
        length++;
    }

    public void writeDouble( double val )
    {
        writeBytes(BitConverter.GetBytes(val));
    }

    public void writeFloat(float val)
    {
        writeBytes(BitConverter.GetBytes(val));
    }

    public void writeInt(int val)
    {
        writeBytes(BitConverter.GetBytes(val));
    }

    public void writeShort(short val)
    {
        writeBytes(BitConverter.GetBytes(val));
    }

    public void writeUnsignedInt(uint val)
    {
        writeBytes(BitConverter.GetBytes(val));
    }

    public void writeUnsignedShort(ushort val)
    {
        writeBytes(BitConverter.GetBytes(val));
    }

    public void writeUTF(string val)
    {
        byte[] b = System.Text.Encoding.UTF8.GetBytes(val);
        writeUnsignedShort((ushort)b.Length);
        writeBytes(b);
    }
}
