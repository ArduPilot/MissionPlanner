using System;
using System.IO;
using System.Drawing;
using MPEGBuilder1UI;

namespace BitmapImage
{
	public class test
    {	
		Bitmap Image = new Bitmap(1920,1080);
	
		private long outBytes = 0;
		public int nPictures = 1;
		
		// Function that converts the image in pictureBox1 to an
		// MPEG format image.  The number of frames to write to the
		// output file is specified by nPictures.
		public void WriteMPEGSequence(string fileName = "out.mpeg")
		{
			int i;
			int j, k1, k2;
			long j2;
			byte tempByte;

			int ACSIZE = 1764;
			byte[] leftoverBits = new byte[10];
			byte[] DCbits = new byte[24];
			byte[] ACbits = new byte[ACSIZE];

			int DCY, DCCR, DCCB, lastDCY, lastDCCR, lastDCCB;
			int hblock, vblock;
			byte[,] Y = new byte[16,16];
			byte[,] CR = new byte[8,8];
			byte[,] CB = new byte[8,8];
			byte[,] block = new byte[8,8];
			double[,] S = new double[8,8];
			int[,] Q = new int[8,8];
			int[] ZZ = new int[64];
			long imageBytes = 0;
			long compressedBytes = 0;
			double compressPercent = 0.0;
			string mBox = null;
			long bitRate;

			MPEGFunctions MPEG = new MPEGFunctions();

			// Retrieve image from pictureBox1
			Bitmap img = new Bitmap(Image);
			imageBytes = img.Height * img.Width * 3;
			
            // Create output file and Memory Stream to write encoded image to
			BinaryWriter bw = new BinaryWriter(File.Create(fileName));
			MemoryStream ms = new MemoryStream();
			FileInfo info = new FileInfo(fileName);

			//	Set up variables to encode image into MPEG frame
			lastDCY = 128;
			lastDCCR = lastDCCB = 128;
			for (i=0; i<10; i++)
				leftoverBits[i] = 255;
			for (i=0; i<24; i++)
				ACbits[i] = 255;
			for (i=0; i<24; i++)
				DCbits[i] = 255;

			outBytes = 20;

			// Write MPEG picture and slice headers to MemoryStream
			for (i=0; i<10; i++)
				MPEG.picHeaderBits[i+32] = (byte) ((0 & (int) Math.Pow (2,9-i)) >> (9-i));
			MPEG.writeToMS(leftoverBits, MPEG.picHeaderBits, ACbits, ref outBytes);
			MPEG.writeToMS(leftoverBits, MPEG.sliceHeaderBits, ACbits, ref outBytes);

			// Do this for each 16x16 pixel block in the bitmap file
			for (vblock=0; vblock<img.Height/16; vblock++)
				for (hblock=0; hblock<img.Width/16; hblock++)
				{
					//	Write 2 bits for Macroblock header to leftoverbits
					//	leftoverbits = '1', '1';
					MPEG.writeMbHeader(leftoverBits);

					//	Fill the Y[] array with a 16x16 block of RGB values
					Y = MPEG.getYMatrix(img, vblock, hblock);
					//	Fill the CR and CB arrays with 8x8 blocks by subsampling 
					//	the RGB array
					CR = MPEG.getCRMatrix(img, vblock, hblock);
					CB = MPEG.getCBMatrix(img, vblock, hblock);

					// First calculate DCTs for the 4 Y blocks
					for (k1=0; k1<2; k1++)
						for (k2=0; k2<2; k2++)
						{
							//	Put 8x8 Y blocks into the block[] array and
							//	then calculate the DCT and quantize the result
							for (i=0; i<8; i++)
								for (j=0; j<8; j++)
									block[i,j] = Y[(k1*8 + i),(k2*8 + j)];
							S = MPEG.calculateDCT(block);
							Q = MPEG.Quantize(S);

							//	Section to differentially Huffman encode DC values
							//	DC is the diffential value for the DC coefficient
							//	lastDC is the running total of the full magnitude
							//	Then send the DC value to DCHuffmanEncode
							for (i=0; i<24; i++)
								DCbits[i] = 255;
							DCY = Q[0,0] - lastDCY;
							lastDCY += DCY;
							DCbits = MPEG.DCHuffmanEncode(DCY, MPEG.DCLumCode, MPEG.DCLumSize);

							//	Section to encode AC Huffman values
							//	Put the AC coefficients into the ACarray[]
							//	in zigzag order, then Huffman encode the
							//	resulting array.
							for (i=0; i<ACSIZE; i++)
								ACbits[i] = 255;
							ZZ = MPEG.Zigzag(Q);
							ACbits = MPEG.ACHuffmanEncode(ZZ);

							//	Write the encoded bits to the MemoryStream
							MPEG.writeToMS(leftoverBits, DCbits, ACbits, ref outBytes);
						}

					// Now calculate the DCT for the CB array and quantize
					S = MPEG.calculateDCT(CB);
					Q = MPEG.Quantize(S);

					//	Encode DC value
					for (i=0; i<24; i++)
						DCbits[i] = 255;
					DCCB = Q[0,0] - lastDCCB;
					lastDCCB += DCCB;
					DCbits = MPEG.DCHuffmanEncode(DCCB, MPEG.DCChromCode, MPEG.DCChromSize);

					//	Encode AC values
					for (i=0; i<ACSIZE; i++)
						ACbits[i] = 255;
					ZZ = MPEG.Zigzag(Q);
					ACbits = MPEG.ACHuffmanEncode(ZZ);

					//	Write the encoded bits to the MemoryStream
					MPEG.writeToMS(leftoverBits, DCbits, ACbits, ref outBytes);

					// Now calculate the DCT for the CR array and quantize
					S = MPEG.calculateDCT(CR);
					Q = MPEG.Quantize(S);
					
					// Encode DC value
					for (i=0; i<24; i++)
						DCbits[i] = 255;
					DCCR = Q[0,0] - lastDCCR;
					lastDCCR += DCCR;
					DCbits = MPEG.DCHuffmanEncode(DCCR, MPEG.DCChromCode, MPEG.DCChromSize);

					//	Encode AC values
					for (i=0; i<ACSIZE; i++)
						ACbits[i] = 255;
					ZZ = MPEG.Zigzag(Q);
					ACbits = MPEG.ACHuffmanEncode(ZZ);

					//	Write the encoded bits to the MemoryStream
					MPEG.writeToMS(leftoverBits, DCbits, ACbits, ref outBytes);
				}

			// Write EOP bits to the MemoryStream
			MPEG.writeEOP(leftoverBits, MPEG.EOPBits);				
			outBytes++;

            //	Put memory stream (which contains the encoded image) into buffer
			ms = MPEG.getMS();
			byte[] buffer = new Byte[ms.Length];
			buffer = ms.ToArray();

			//	Set MPEG Sequence Header bits to correct image size
			j = 2048;
			for (i=0; i<12; i++)
			{
				MPEG.seqHeaderBits[i+32] = (byte) ((j&img.Width) >> (11-i));
				MPEG.seqHeaderBits[i+44] = (byte) ((j&img.Height) >> (11-i));
				j >>= 1;
			}

			//	Set MPEG Sequence Header bits to bitRate value
			bitRate = ms.Length * 30 * 8 / 400;
			j2 = 131072;
			for (i=0; i<18; i++)
			{
				MPEG.seqHeaderBits[i+64] = (byte) ((j2&bitRate) >> (17-i));
				j2 >>= 1;
			}

			//	Write MPEG Sequence header to file
			for (i=0; i<12; i++)
			{
				tempByte = 0;
				for (j=0; j<8; j++)
					tempByte = (byte) (tempByte*2 + MPEG.seqHeaderBits[i*8 + j]);
				bw.Write(tempByte);
			}
	
			//	Write MPEG GOP header to file
			for (i=0; i<8; i++)
			{
				tempByte = 0;
				for (j=0; j<8; j++)
					tempByte = (byte) (tempByte*2 + MPEG.GOPHeaderBits[i*8 + j]);
				bw.Write(tempByte);
			}

			//	Fix the picture header for each MPEG frame and write 
			//	the buffer to the file
			for (i=0; i<nPictures; i++)
			{
				for (j=0; j<10; j++)
					MPEG.picHeaderBits[j+32] = (byte) ((i & (int) Math.Pow (2,9-j)) >> (9-j));
				for (j=0; j<4; j++)
				{
					tempByte = 0;
					for (k1=0; k1<8; k1++)
						tempByte = (byte) (2*tempByte + MPEG.picHeaderBits[j*8 + k1]);
					buffer[j] = tempByte;
				}
				bw.Write(buffer);
			}

			// Write the End Of Sequence header
			bw.Write((byte) 0x00);
			bw.Write((byte) 0x00);
			bw.Write((byte) 0x01);
			bw.Write((byte) 0xb7);
			bw.Close();

			// Calculate some output statistics
			outBytes += 4;
			compressedBytes = outBytes - 20 - 8 - 4;
			compressPercent = 100.0 - (double) (compressedBytes * 100.0 / imageBytes);

			if (nPictures == 1)
			{
				mBox = "Original image bytes (24 color bitmap): " + imageBytes.ToString() + "\n";
				mBox += "Compressed image bytes: " + compressedBytes.ToString() + "\n";
				mBox += "Compression Percentage: " + compressPercent.ToString();
			}
			else
			{
				//mBox += "Input File Size: " + inputFileLength.ToString() + "\n";
				mBox += "Output File Size: " + info.Length.ToString() + "\n";
			}

			Console.WriteLine(mBox);
			//MessageBox.Show(mBox, "Compression Statistics");
		}
    }


	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class MPEGFunctions
	{
		// Class of functions and variables needed to encode images
		// using the MPEG1 format
		// The MemoryStream ms is provided to store the bitstream 
		// produced by encoding the image's blocks
		private MemoryStream ms = new MemoryStream();
		private double[,] cosine = new double[8,8];
		private double SQRT2o2 = Math.Sqrt(2.0) / 2.0;

		// MPEG1 default quantization matrix
		private byte[,] defaultQ = new byte[8,8] {{8,16,19,22,26,27,29,34},
										{16,16,22,24,27,29,34,37},
										{19,22,26,27,29,34,34,38},
										{22,22,26,27,29,34,37,40},
										{22,26,27,29,32,35,40,48},
										{26,27,29,32,35,40,48,58},
										{26,27,29,34,38,46,56,69},
										{27,29,35,38,46,56,69,83}};

		// MPEG1 default DC Huffman codes 
		public int[] DCLumCode = new int[9] {4, 0, 1, 5, 6, 14, 30, 62, 126};
		public int[] DCLumSize = new int[9] {3, 2, 2, 3, 3, 4, 5, 6, 7};
		public int[] DCChromCode = new int[9] {0, 1, 2, 6, 14, 30, 62, 126, 254};
		public int[] DCChromSize = new int[9] {2, 2, 2, 3, 4, 5, 6, 7, 8};

		// MPEG1 default AC Huffman codes
		private int[] ACcode = new int[111] {
		6,6,10,14,12,14,10,8,14,10,78,70,68,64,28,26,16,	// AC=1, run=0->16    
		62,52,50,46,44,62,60,58,56,54,62,60,58,56,54,		// AC=1, run=17->31   
		8,12,8,72,30,18,60,42,34,34,32,52,50,48,46,44,42,	// AC=2, run=0->16   
		10,74,22,56,36,36,40,		// AC=3, run=0->6		
		12,24,40,38,				// AC=4, run=0->3	
		76,54,40,				// AC=5, run=0->2	
		66,44,					// AC=6	
		20,42,
		58,62,
		48,60,
		38,58,
		32,56,
		52,54,
		50,52,
		48,50,
		46,38,
		62,36,
		62,34,
		58,32,
		56,54,52,50,48,46,44,42,40,38,36,34,32,		// AC=19->31, run=0	
		48,46,44,42,40,38,36,34,32};			// AC=32->40, run=0

		private int[] ACsize = new int[111] {
		3,4,5,6,6,7,7,7,8,8,9,9,9,9,11,11,11,	// AC=1, run=0->16
		13,13,13,13,13,14,14,14,14,14,17,17,17,17,17,	// AC=1, run=17->31
		5,7,8,9,11,11,13,13,13,14,14,17,17,17,17,17,17,	// AC=2, run=0->16
		6,9,11,13,13,14,17,				// AC=3, run=0->6
		8,11,13,14,					// AC=4, run=0->3
		9,13,14,					
		9,14,
		11,14,
		13,16,
		13,16,
		13,16,
		13,16,
		14,16,
		14,16,
		14,16,
		14,17,
		15,17,
		15,17,
		15,17,
		15,15,15,15,15,15,15,15,15,15,15,15,15,				// AC=19->31, run=0
		16,16,16,16,16,16,16,16,16};					// AC=32->40, run=0	

		// MPEG1 sequence header
		public byte[] seqHeaderBits = new byte[100] 
				  {
						  0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
					  0,0,0,0,0,0,0,1,1,0,1,1,0,0,1,1,	// Sequence Header
					  0,0,0,1,0,1,1,0,0,0,0,0,		// HSize = 352
					  0,0,0,0,1,1,1,1,0,0,0,0,		// VSize = 240
					  1,1,0,0,0,1,0,0,		// Pel Shape=1.125,Picture Rate=29.97
					  0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,	// Bit Rate = 4096*400
					  1,						// Marker Bit
					  0,0,0,0,0,1,0,1,0,0,			// VBV Buffer Size = 20 * 16384
					  0,0,0,255,255,255,255};

		// MPEG1 GOP header
		public byte[] GOPHeaderBits = new byte[68]
				{
						0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
					0,0,0,0,0,0,0,1,1,0,1,1,1,0,0,0,	// GOP Header
					1,0,0,0,0,0,0,0,0,0,0,0,
					1,0,0,0,0,0,0,0,0,0,0,0,0,		// Time Code = 00:00:00
					1,0,0,0,0,0,0,255,255,255,255};		// Closed GOP, Broken Link
		
		// MPEG1 picture header
		public byte[] picHeaderBits = new byte[68]
				{
						0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
					0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,	// Picture Header
					0,0,0,0,0,0,0,0,0,0,			// Temporal Reference
					0,0,1,					// Picture Type = I Frame
					1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,	// VBV Delay = 32768
					0,0,0,255,255,255,255};
		
		// MPEG1 slice header
		public byte[] sliceHeaderBits = new byte[44]
				{
						0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
					0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,	// Slice Header
					0,1,0,0,0,					// Q Scale = 8
					0,255,255,255,255,255,255};				// Extra Slice Bit

		public byte[] EOPBits = new byte[8] {255,255,255,255,255,255,255,255};

		public MPEGFunctions()
		{
			int i, j;

			// Calculate frequently used cosine matrix
			for (i=0; i<8; i++)
				for (j=0; j<8; j++)
					cosine[j,i] = Math.Cos(Math.PI*j*(2.0*i + 1)/16.0);
		}

		//	The memory stream can be used to store the encoded picture
		//	by using the writeToMS function anytime a block is encoded
		public MemoryStream getMS()
		{
			return ms;
		}

		//	Perform the DCT on the A block
		//	See the JPEG DCT definition for the formula
		public double[,] calculateDCT (byte[,] A)
			{
			int k1, k2, i, j;
			double Cu, Cv;
			double[,] B = new double[8,8];

			for (k1=0; k1<8; k1++)
				for (k2=0; k2<8; k2++)
				{
					B[k1,k2] = 0.0;
					for (i=0; i<8; i++)
						for (j=0; j<8; j++)
							B[k1,k2] += A[i,j] * cosine[k1,i] * cosine[k2,j];

					if (k1==0)
						Cu = SQRT2o2;
					else
						Cu = 1.0;
					if (k2==0)
						Cv = SQRT2o2;
					else
						Cv= 1.0;
					
					B[k1,k2] *= (0.25*Cu*Cv);
				}
			return B;	// Return Frequency Component matrix
		}

		// Quantize the DCT coefficients using the defaultQ array
		// In MPEG, always divide the DC value by eight
		public int[,] Quantize (double[,] S)
		{
			int i, j;
			int[,] S1 = new int[8,8];

			for (i=0; i<8; i++)
				for (j=0; j<8; j++)
					S1[i,j] = (int) Math.Round((S[i,j] / (double) defaultQ[i,j]));

			return S1;	// Return quantized Frequency Component matrix
		}			

			
		// Quantize the DCT coefficients using the supplied Q array
		// In MPEG, always divide the DC value by eight
		public int[,] Quantize (double[,] S, double[,] Q)
		{
			int i, j;
			int[,] S1 = new int[8,8];

			for (i=0; i<8; i++)
				for (j=0; j<8; j++)
					S1[i,j] = (int) Math.Round((S[i,j] / Q[i,j]));

			return S1;	// Return quantized Frequency Component matrix
		}			


		//	Function to encode the DC values
		public byte[] DCHuffmanEncode(int DC, int[] eco, int[] esi)
		{
			byte[] DCbits = new byte[24];
			int cat, tempval, i;
			bool invert;
			int tempbits;

			for (i=0; i<24; i++)
				DCbits[i] = 255;

			// Set flag if DC is negative
			invert = false;
			if (DC<0)
				invert = true;
			DC = Math.Abs(DC);

			// Determine the number of bits needed to represent DC
			cat = 0;
			tempval = DC;
			while (tempval >= 1)
			{
				cat++;
				tempval >>= 1;
			}

			// Write bits for Huffman code into DCbits[] array
			tempval = (int) Math.Pow(2, esi[cat]-1);
			for (i=0; i<esi[cat]; i++)
			{
				tempbits = eco[cat] & tempval;
				DCbits[i] = (byte) (tempbits >> (esi[cat] - 1 - i));
				tempval >>= 1;
			}

			// Write bits for DC into DCbits[] array
			tempval = (int) Math.Pow(2, cat-1);
			for (i=esi[cat]; i<esi[cat]+cat; i++)
			{
				tempbits = DC & tempval;
				DCbits[i] = (byte) (tempbits >> (cat-1-i+esi[cat]));
				tempval >>= 1;
			}

			// If necessary, invert bits to represent negative DC
			if (invert)
				for (i=esi[cat]; i<esi[cat]+cat; i++)
				{
					if (DCbits[i] == 0)
						DCbits[i] = 1;
					else
						DCbits[i] = 0;
				}

			return DCbits;	// Return array of bits representing DC value
		}


		//	Re order all coefficients after DCT is performed
		public int[] Zigzag (int[,] S1)
		{
			int[] index = new int[64] {0,1,7,8,-7,-7,1,7,7,7,8,
							 -7,-7,-7,-7,1,7,7,7,7,7,8,
							 -7,-7,-7,-7,-7,-7,1,7,7,7,7,7,7,7,1,
							 -7,-7,-7,-7,-7,-7,8,7,7,7,7,7,1,
							 -7,-7,-7,-7,8,7,7,7,1,
							 -7,-7,8,7,1};
			int i;
			int[] zz = new int[64];
			int a, b;
			int S1pointer = 0;

			for (i=0; i<64; i++)
			{
				S1pointer += index[i];
				a = S1pointer/8;
				b = S1pointer%8;
				zz[i] = S1[a,b];
			}

			return zz;	// Return sorted array with DC value at position 0
		}


		//	This function takes the array pointed to by zz and encodes
		//	the values using the Huffman code pointed to by eco[].
		public byte[] ACHuffmanEncode(int[] zz)
		{
			int MAXACSIZE = 1764;	// 63 AC Values * 28 bits
			byte[] outBits = new byte[MAXACSIZE];
			int run, tempval;
			int ACindex = 0;
			int i, AC;
			int arrayPosition;
			int ACbits;
			int size;
			int code;
			int outbitsPosition;
			int tempbits;

			for (i=0; i<MAXACSIZE; i++)
				outBits[i] = 255;

			//	Start at zz[1] since don't want to include DC value
			//	Do this to the end of the zz array
			arrayPosition = 1;
			outbitsPosition = 0;
			while (ACindex < MAXACSIZE - 28)
			{
				// First figure out how many consecutive zeros
				run = 0;
				while ((zz[arrayPosition]==0) && (arrayPosition<63))
				{
					run++;
					arrayPosition++;
				}

				// Read in the AC value after the zeros
				AC = zz[arrayPosition];
				arrayPosition++;

				//	Set up exit condition from loop if at end of block
				if (arrayPosition >= 63)
				{
					//	Reset other values
					AC = 0;
					run = 0;
					ACbits = ACindex;
					ACindex = MAXACSIZE + 200;
				}

				//	Determine Huffman code and number of bits needed to
				//	represent the run,level combination.  See MPEG1 spec.
				if ((run<=31) && (Math.Abs(AC)==1))
				{
					code=ACcode[run];
					size=ACsize[run];
					//	If AC is negative, the sign bit in the Huffman code
					//	equals 1, else it equals 0. So if negative, add 1 to code.
					if (AC < 0)
						code += 1;
				}
				else if ((run<=16) && (Math.Abs(AC)==2))
				{
					code=ACcode[32+run];
					size=ACsize[32+run];
					if (AC < 0)
						code += 1;
				}
				else if ((run<=6) && (Math.Abs(AC)==3))
				{
					code=ACcode[49+run];
					size=ACsize[49+run];
					if (AC < 0)
						code += 1;
				}
				else if ((run<=3) && (Math.Abs(AC)==4))
				{
					code=ACcode[56+run];
					size=ACsize[56+run];
					if (AC < 0)
						code += 1;
				}
				else if ((run<=2) && (Math.Abs(AC)==5))
				{
					code=ACcode[60+run];
					size=ACsize[60+run];
					if (AC < 0)
						code += 1;
				}
				else if ((run<=1) && ((Math.Abs(AC)>=6) && (Math.Abs(AC)<=18)))
				{
					code=ACcode[63+run+((Math.Abs(AC)-6)*2)];
					size=ACsize[63+run+((Math.Abs(AC)-6)*2)];
					if (AC < 0)
						code += 1;
				}
				else if ((run==0) && ((Math.Abs(AC)>=19) && (Math.Abs(AC)<=40)))
				{
					code=ACcode[89+Math.Abs(AC)-19];
					size=ACsize[89+Math.Abs(AC)-19];
					if (AC < 0)
						code += 1;
				}
				else if ((run==0) && (AC==0))	// EOB condition
				{
					code = 2;
					size = 2;
				}
				else
				{
					code = escapecode(run, AC);
					if (Math.Abs(AC) >= 128)
						size = 28;
					else
						size = 20;
				}

				// Write bits for Huffman code into bits[] array
				tempval = (int) Math.Pow(2,size-1);
				for (i=0; i<size; i++)
				{
					tempbits = code & tempval;
					outBits[i+outbitsPosition]  = (byte) (tempbits >> (size-i-1));
					tempval >>= 1;
				}

				outbitsPosition += size;

				// Increase index for bits array (don't exceed array size) 
				ACindex += size;
			}

			return outBits;
		}
	
		//	Function that calculates the Huffman code value for MPEG run,level
		//	combinations that don't have defined codes.
		private int escapecode(int run, int AC)
		{
			int intval = 0;
			int code = 0;

			//	Last 8 or 16 bits in the code are the AC value
			//	For positve values, use normal binary encoding.
			//	For negative values from -1 to -127, use 2's complement value.
			//	For negative values from -128 to -255, use 16 bit value with MSB=1,
			//	and 2's complement of AC.
			if (AC > 0)
				intval = AC;
			else
			{
				if (AC >= -127)
					intval = 256 + AC;
				else 
					intval = 32768 + 256 + AC;
			}

			//	Construct escape code using 000001 as first 6 bits, 
			//	binary representation of run as next 6 bits,
			//	and either 8 or 16 bits representation of AC from above.
			if (Math.Abs(AC) < 128)
				code = 16384 + 256*run + intval;
			else
				code = 4194304 + 65536*run + intval;

			return (code);
		}

		// Write the Macroblock header for I frame blocks
		public void writeMbHeader(byte[] left)
		{
			int i = 0;

			//	Search leftoverBits[] for first -1 value
			while (left[i] !=  255)
				i++;

			//	Write MB Header values '1 1' to leftoverBits[]
			left[i++] = 1;
			left[i] = 1;
		}

		//	Function to take the encoded bits from the latest block and
		//	write them to a memory stream
		public void writeToMS(byte[] left, byte[] DC, byte[] AC, ref long outBytes)
			{
			int i, j, bytevalue, leftBits;
			byte[] bitArray = new byte[2100];
			int bitIndex = 0;
			int outputbytes = 0;

			// The worst case is 63 AC values that are each 28 bits long 
			// plus a 24 bit DC value
			for (i=0; i<2100; i++)
				bitArray[i] = 0;

			//	Write the leftover bits from the previous block to bitArray[]
			i = 0;
			while (left[i] != 255)
				bitArray[bitIndex++] = left[i++];

			//	Write the DC bits from the current block to bitArray[]
			i = 0;
			while (DC[i] != 255)
				bitArray[bitIndex++] = DC[i++];

			//	Write the AC bits from the current block to bitArray[]
			i = 0;
			while (AC[i] != 255)
				bitArray[bitIndex++] = AC[i++];

			//	Calculate the number of bytes to write to the MemoryStream and 
			//	how many bits will be left over
			outputbytes = bitIndex / 8;
			leftBits = bitIndex % 8;

			//	Write the bytes to the MemoryStream
			for (i=0; i<outputbytes; i++)
			{
				bytevalue = 0;
				for (j=0; j<8; j++)
					bytevalue += (int) ((bitArray[i*8 + j] * Math.Pow(2,(7-j))));
				ms.WriteByte((byte) bytevalue);
			}

			//	Store the leftover bits in left[] and fill the rest with -1's
			for (i=0; i<leftBits; i++)
				left[i] = bitArray[outputbytes*8 + i];
			for (i=leftBits; i<10; i++)
				left[i] = 255;

			outBytes += outputbytes;
		}

		//	Function to write the final leftover bits to the memoryStream and
		//	then stuff with 0's to byte align for next picture
		public void writeEOP(byte[] left, byte[] EOP)
		{
			int i;
			int j, leftBits;
			int bytevalue;
			byte[] bitArray = new byte[20];
			int bitIndex = 0;
			int outputbytes = 0;

			for (i=0; i<20; i++)
				bitArray[i] = 0;

			//	Write the leftover bits from the previous block to bitArray[]
			i=0;
			while (left[i] != 255)
				bitArray[bitIndex++] = left[i++];

			//	Write the EOP bits from the current block to bitArray[]
			i=0;
			while (EOP[i] != 255)
				bitArray[bitIndex++] = EOP[i++];

			//	Calculate the number of bytes to write to the MemoryStream and 
			//	how many bits will be left over
			outputbytes = bitIndex / 8;
			leftBits = bitIndex % 8;

			//	If one full byte needs to be written
			if (outputbytes == 1)
			{
				bytevalue = 0;
				for (j=0; j<8; j++)
					bytevalue = (byte) (2*bytevalue + bitArray[j]);
				ms.WriteByte((byte) bytevalue);
			}
				//	Else pad leftover bits with zeros and write to MemoryStream
			else
			{
				bytevalue = 0;
				for (j=0; j<8; j++)
					bytevalue = (byte) (2*bytevalue + bitArray[j]);
				ms.WriteByte((byte) bytevalue);
			}

			//	Leftover is cleared so re-initialize leftover bits to -1's
			for (j=0; j<10; j++)
				left[j] = 255;
		}

		// Function to get the Y values for an RGB macroblock
		// pointed to by [vblock,hblock] out of Bitmap img
		public byte[,] getYMatrix(Bitmap img, int vblock, int hblock)
		{
			int i, j;
			double tempdouble;
			byte[,] Y = new byte[16,16];

			for (i=0; i<16; i++)
				for (j=0; j<16; j++)
				{
					tempdouble = (219.0*(0.59*img.GetPixel((hblock*16 + j),(vblock*16 + i)).R +
						0.30*img.GetPixel((hblock*16 + j),(vblock*16 + i)).G + 
						0.11*img.GetPixel((hblock*16 + j),(vblock*16 + i)).B) / 255.0) + 16.0;
					Y[i,j] = (byte) (Math.Round(tempdouble));	// Y is limited from 16 to 235
				}
			return Y;
		}

		// Function to get the CR values for an RGB macroblock
		// pointed to by [vblock,hblock] out of Bitmap img
		// In MPEG1, use subsampling to make one 8x8 block of Pr values
		public byte[,] getCRMatrix(Bitmap img, int vblock, int hblock)
		{
			int i, j;
			double tempdouble;
			byte[,] CR = new byte[8,8];

			for (i=0; i<8; i++)
				for (j=0; j<8; j++)
				{
					tempdouble = (224.0*(0.50*img.GetPixel((hblock*16 + j*2),(vblock*16 + i*2)).R -
						0.42*img.GetPixel((hblock*16 + j*2),(vblock*16 + i*2)).G - 
						0.08*img.GetPixel((hblock*16 + j*2),(vblock*16 + i*2)).B) / 255.0) + 128.0;
					CR[i,j] = (byte) (Math.Round(tempdouble));	// Cr is limited from 16 to 235
				}
			return CR;
		}

		// Function to get the CB values for an RGB macroblock
		// pointed to by [vblock,hblock] out of Bitmap img
		// In MPEG1, use subsampling to make one 8x8 block of Pb values
		public byte[,] getCBMatrix(Bitmap img, int vblock, int hblock)
		{
			int i, j;
			double tempdouble;
			byte[,] CB = new byte[8,8];

			for (i=0; i<8; i++)
				for (j=0; j<8; j++)
				{
					tempdouble = (224.0*(-0.17*img.GetPixel((hblock*16 + j*2),(vblock*16 + i*2)).R -
						0.33*img.GetPixel((hblock*16 + j*2),(vblock*16 + i*2)).G + 
						0.50*img.GetPixel((hblock*16 + j*2),(vblock*16 + i*2)).B) / 255.0) + 128.0;
					CB[i,j] = (byte) (Math.Round(tempdouble));	// Cb is limited from 16 to 235
				}
			return CB;
		}
	}
}
