using System;
using UnityEngine;


namespace InControl
{
	public class InputControlMapping
	{
		public class Range
		{
			public static Range Complete = new Range { Minimum = -1.0f, Maximum = 1.0f };
			public static Range Positive = new Range { Minimum =  0.0f, Maximum = 1.0f };
			public static Range Negative = new Range { Minimum = -1.0f, Maximum = 0.0f };

			public float Minimum;
			public float Maximum;
		}


		public InputControlSource Source;
		public InputControlType Target;

		public bool Invert;
		public bool Normal;

		// Raw inputs won't be range remapped, smoothed or filtered.
		public bool Raw;

		public Range SourceRange = Range.Complete;
		public Range TargetRange = Range.Complete;

		string handle;


		public float MapValue( float value )
		{
			float targetValue;

			if (Raw)
			{
				targetValue = value;
			}
			else
			{
//				var minimum = SourceRange.Minimum == 0.0f ?  float.Epsilon : SourceRange.Minimum;
//				var maximum = SourceRange.Maximum == 0.0f ? -float.Epsilon : SourceRange.Minimum;

				if (Mathf.Abs(value) < float.Epsilon)
				{
					targetValue = 0.0f;
				}
				else
				{
					if (value < SourceRange.Minimum || value > SourceRange.Maximum)
					{
						// Values outside of source range are considered invalid.
						return 0.0f;
					}

					var sourceValue = Mathf.InverseLerp( SourceRange.Minimum, SourceRange.Maximum, value );
					targetValue = Mathf.Lerp( TargetRange.Minimum, TargetRange.Maximum, sourceValue );
				}
			}

			// Invert value if necessary.
			if (Invert ^ (IsYAxis && InputManager.InvertYAxis))
			{
				targetValue = -targetValue;
			}

			// Normal means value must be -1, 0 or 1
			if (Normal && Math.Abs(targetValue) > float.Epsilon)
			{
				targetValue = Mathf.Sign( targetValue );
			}

			return targetValue;
		}


		public bool TargetRangeIsNotComplete
		{
			get { return TargetRange != Range.Complete; }
		}


		public string Handle
		{
			get { return (string.IsNullOrEmpty( handle )) ? Target.ToString() : handle; }
			set { handle = value; }
		}


		bool IsYAxis
		{
			get
			{
				return Target == InputControlType.LeftStickY   ||
					   Target == InputControlType.RightStickY;
			}
		}
	}
}
