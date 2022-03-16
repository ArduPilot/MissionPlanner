namespace Rtsp.Messages
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;

    /// <summary>
    /// Describe a couple of port used to transfer video and command.
    /// </summary>
    public class PortCouple
    {
        /// <summary>
        /// Gets or sets the first port number.
        /// </summary>
        /// <value>The first port.</value>
        public int First { get; set; }
        /// <summary>
        /// Gets or sets the second port number.
        /// </summary>
        /// <remarks>If not present the value is 0</remarks>
        /// <value>The second port.</value>
        public int Second { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PortCouple"/> class.
        /// </summary>
        public PortCouple()
        { }
        /// <summary>
        /// Initializes a new instance of the <see cref="PortCouple"/> class.
        /// </summary>
        /// <param name="first">The first port.</param>
        public PortCouple(int first)
        {
            First = first;
            Second = 0;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PortCouple"/> class.
        /// </summary>
        /// <param name="first">The first port.</param>
        /// <param name="second">The second port.</param>
        public PortCouple(int first, int second)
        {
            First = first;
            Second = second;
        }

        /// <summary>
        /// Gets a value indicating whether this instance has second port.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has second port; otherwise, <c>false</c>.
        /// </value>
        public bool IsSecondPortPresent
        {
            get { return Second != 0; }
        }

        /// <summary>
        /// Parses the int values of port.
        /// </summary>
        /// <param name="stringValue">A string value.</param>
        /// <returns>The port couple</returns>
        public static PortCouple Parse(string stringValue)
        {
            if (stringValue == null)
                throw new ArgumentNullException("stringValue");
            Contract.Requires(!string.IsNullOrEmpty(stringValue));

            string[] values = stringValue.Split('-');

            int tempValue;

            int.TryParse(values[0], out tempValue);
            PortCouple result = new PortCouple(tempValue);

            tempValue = 0;
            if (values.Length > 1)
                int.TryParse(values[1], out tempValue);

            result.Second = tempValue;

            return result;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (IsSecondPortPresent)
                return First.ToString(CultureInfo.InvariantCulture) + "-" + Second.ToString(CultureInfo.InvariantCulture);
            else
                return First.ToString(CultureInfo.InvariantCulture);
        }


    }
}
