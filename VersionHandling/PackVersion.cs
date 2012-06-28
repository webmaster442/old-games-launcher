using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VersionHandling
{
    [Serializable]
    public class PackVersion: IComparable<PackVersion>
    {
        public short Year { get; set; }
        public byte Month { get; set; }
        public byte Date { get; set; }
        public byte Hour { get; set; }
        public byte Minute { get; set; }
        public byte Second { get; set; }

        public PackVersion() { }

        public PackVersion(DateTime source)
        {
            this.FromDateTime(source);
        }

        public void FromDateTime(DateTime t)
        {
            Year = (short)t.Year;
            Month = (byte)t.Month;
            Date  = (byte)t.Day;
            Hour = (byte)t.Hour;
            Minute = (byte)t.Minute;
            Second = (byte)t.Second;
        }

        public override int GetHashCode()
        {
            return Year.GetHashCode() ^ Month.GetHashCode() ^ Date.GetHashCode() ^ Hour.GetHashCode() ^ Minute.GetHashCode() ^ Second.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj is PackVersion) return obj.GetHashCode() == this.GetHashCode();
            else return false;
        }

        public DateTime ToDateTime()
        {
            return new DateTime(Year, Month, Date, Hour, Minute, Second);
        }

        public int CompareTo(PackVersion other)
        {
            if (other == null) throw new ArgumentException("other can't be null");
            return this.ToDateTime().CompareTo(other.ToDateTime());
        }

        public static bool operator <(PackVersion i1, PackVersion i2)
        {
            int val = i1.CompareTo(i2);
            return val < 0;
        }

        public static bool operator >(PackVersion i1, PackVersion i2)
        {
            int val = i1.CompareTo(i2);
            return val > 0;
        }

        public static bool operator ==(PackVersion i1, PackVersion i2)
        {
            return i1.Equals(i2);
        }

        public static bool operator !=(PackVersion i1, PackVersion i2)
        {
            return !i1.Equals(i2);
        }
    }
}
