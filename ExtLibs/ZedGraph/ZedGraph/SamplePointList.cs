using System;
using System.Collections;
using System.Text;

namespace ZedGraph
{
	/// <summary>
	/// enumeration used to indicate which type of data will be plotted.
	/// </summary>
	public enum SampleType
	{
		/// <summary>
		/// Designates the "Time" property will be used
		/// </summary>
		Time,
		/// <summary>
		/// Designates the "Position" property will be used
		/// </summary>
		Position,
		/// <summary>
		/// Designates the Instantaneous Velocity property will be used
		/// </summary>
		VelocityInst,
		/// <summary>
		/// Designates the "Time since start" property will be used
		/// </summary>
		TimeDiff,
		/// <summary>
		/// Designates the Average Velocity property will be used
		/// </summary>
		VelocityAvg
	};

	/// <summary>
	/// A simple storage class to maintain an individual sampling of data
	/// </summary>
	public class Sample : System.Object
	{
		private DateTime	_time;
		private double		_position;
		private double		_velocity;

		/// <summary>
		/// The time of the sample
		/// </summary>
		public DateTime Time
		{
			get { return _time; }
			set { _time = value; }
		}
		/// <summary>
		/// The position at sample time
		/// </summary>
		public double Position
		{
			get { return _position; }
			set { _position = value; }
		}
		/// <summary>
		/// The instantaneous velocity at sample time
		/// </summary>
		public double Velocity
		{
			get { return _velocity; }
			set { _velocity = value; }
		}
	}

	/// <summary>
	/// A collection class to maintain a set of samples
	/// </summary>
	[Serializable]
	public class SamplePointList : IPointList
	{
		/// <summary>
		/// Determines what data type gets plotted for the X values
		/// </summary>
		public SampleType XType;
		/// <summary>
		/// Determines what data type gets plotted for the Y values
		/// </summary>
		public SampleType YType;

		// Stores the collection of samples
		private ArrayList list;

		/// <summary>
		/// Indexer: get the Sample instance at the specified ordinal position in the list
		/// </summary>
		/// <param name="index">The ordinal position in the list of samples</param>
		/// <returns>Returns a <see cref="PointPair" /> instance containing the
		/// data specified by <see cref="XType" /> and <see cref="YType" />
		/// </returns>
		public PointPair this[int index]
		{
			get
			{
				PointPair pt = new PointPair();
				Sample sample = (Sample) list[index];
				pt.X = GetValue( sample, XType );
				pt.Y = GetValue( sample, YType );
				return pt;
			}
		}

		/// <summary>
		/// Gets the number of samples in the collection
		/// </summary>
		public int Count
		{
			get { return list.Count; }
		}

		/// <summary>
		/// Get the specified data type from the specified sample
		/// </summary>
		/// <param name="sample">The sample instance of interest</param>
		/// <param name="type">The data type to be extracted from the sample</param>
		/// <returns>A double value representing the requested data</returns>
		public double GetValue( Sample sample, SampleType type )
		{
			switch ( type )
			{
				case SampleType.Position:
					return sample.Position;
				case SampleType.Time:
					return sample.Time.ToOADate();
				case SampleType.TimeDiff:
					return sample.Time.ToOADate() - ( (Sample)list[0] ).Time.ToOADate();
				case SampleType.VelocityAvg:
					double timeDiff = sample.Time.ToOADate() - ( (Sample)list[0] ).Time.ToOADate();
					if ( timeDiff <= 0 )
						return PointPair.Missing;
					else
						return ( sample.Position - ( (Sample)list[0] ).Position ) / timeDiff;
				case SampleType.VelocityInst:
					return sample.Velocity;
				default:
					return PointPair.Missing;
			}
		}

		/// <summary>
		/// Append a sample to the collection
		/// </summary>
		/// <param name="sample">The sample to append</param>
		/// <returns>The ordinal position at which the sample was added</returns>
		public int Add( Sample sample )
		{
			return list.Add( sample );
		}

		// generic Clone: just call the typesafe version
		object ICloneable.Clone()
		{
			return this.Clone();
		}

		/// <summary>
		/// typesafe clone method
		/// </summary>
		/// <returns>A new cloned SamplePointList.  This returns a copy of the structure,
		/// but it does not duplicate the data (it just keeps a reference to the original)
		/// </returns>
		public SamplePointList Clone()
		{
			return new SamplePointList( this );
		}

		/// <summary>
		/// default constructor
		/// </summary>
		public SamplePointList()
		{
			XType = SampleType.Time;
			YType = SampleType.Position;
			list = new ArrayList();
		}

		/// <summary>
		/// copy constructor -- this returns a copy of the structure,
		/// but it does not duplicate the data (it just keeps a reference to the original)
		/// </summary>
		/// <param name="rhs">The SamplePointList to be copied</param>
		public SamplePointList( SamplePointList rhs )
		{
			XType = rhs.XType;
			YType = rhs.YType;

			// Don't duplicate the data values, just copy the reference to the ArrayList
			this.list = rhs.list;

			//foreach ( Sample sample in rhs )
			//	list.Add( sample );
		}

	}
}
