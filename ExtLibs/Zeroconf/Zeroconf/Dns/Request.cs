using System;
using System.Collections.Generic;
using System.Text;

namespace Heijden.DNS
{
    class Request
	{
		public Header header;

        List<Question> questions;

		public Request()
		{
			header = new Header();
			header.OPCODE = OPCode.Query;
			header.QDCOUNT = 0;

			questions = new List<Question>();
		}

		public void AddQuestion(Question question)
		{
			questions.Add(question);
		}

		public byte[] Data
		{
			get
			{
				List<byte> data = new List<byte>();
				header.QDCOUNT = (ushort)questions.Count;
				data.AddRange(header.Data);
				foreach (Question q in questions)
					data.AddRange(q.Data);
				return data.ToArray();
			}
		}
	}
}
