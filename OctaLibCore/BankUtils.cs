namespace OctaLib
{
    public class BankUtils
    {
        // TODO: Consider passing in bank data to a constructor vs every method

        /// <summary>
        /// Checks if the header data for this bank file is valid
        /// </summary>
        /// <param name="bankData">Bank byte data</param>
        /// <returns>True if header is valid</returns>
        public static bool ValidateHeader(byte[] bankData)
        {
            for (int i = 0; i < Constants.HEADER_BANK.Length; i++) 
            {
                if (bankData[i] !=  Constants.HEADER_BANK[i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Read the part names for this bank
        /// </summary>
        /// <param name="bankData">Bank byte data</param>
        /// <param name="addr">Address offset</param>
        /// <returns>Part name</returns>
        public static string ReadPartName(byte[] bankData, int partNum)
        {

            char[] partName = new char[6];

            // Part names can be 6 chars
            Array.Copy(bankData, Constants.ADDR_PART_NAME[partNum], partName, 0, 6);

            // But we need to ignore anything after 00 because it might be junk
            for (int i = 0; i < 6; i++)
            {
                if (partName[i] == 0)
                {
                    char[] temp = new char[i];
                    Array.Copy(partName, temp, i);
                    return new String(temp);
                }
            }

            return new String(partName);
        }

        /// <summary>
        /// This doesn't do anything interesting. It just gets the track number for a given track (this should always match its position)
        /// </summary>
        /// <param name="bankData">Bank byte data</param>
        /// <param name="trackNum">Track number (base zero)</param>
        /// <returns>True if content is found</returns>
        public static int ReadTrackNum(byte[] bankData, int trackNum)
        {
            return bankData[Constants.ADDR_PAT01 + Constants.LENGTH_PATTERN_HEADER + 
                (Constants.LENGTH_TRAC * trackNum) + Constants.OFFSET_TRACK_NUM];
        }

        /// <summary>
        /// Determines if a track has content
        /// </summary>
        /// <param name="bankData">Bank byte data</param>
        /// <param name="patNum">Pattern number (base zero)</param>
        /// <param name="trackNum">Track number (base zero)</param>
        /// <returns>True if content is found</returns>
        public static bool ReadTrackState(byte[] bankData, int patNum, int trackNum)
        {

            // Check for regular trigs
            for (int i = 0; i < 8; i++)
            {
                if (bankData[
                    Constants.ADDR_PAT01 + // Address of pattern 1
                    (Constants.LENGTH_PATTERN_LENGTH * patNum) + // Offset by length of a pattern block * pattern number
                    Constants.LENGTH_PATTERN_HEADER + (Constants.LENGTH_TRAC * trackNum) + Constants.OFFSET_TRACK_TRIGS + i] > 0)
                {
                    return true;
                }
            }

            // Check for rec trigs
            for (int i = 0; i < 8; i++)
            {
                if (bankData[
                    Constants.ADDR_PAT01 + // Address of pattern 1
                    (Constants.LENGTH_PATTERN_LENGTH * patNum) + // Offset by length of a pattern block * pattern number
                    Constants.LENGTH_PATTERN_HEADER + (Constants.LENGTH_TRAC * trackNum) + Constants.OFFSET_TRACK_REC_TRIGS + i] > 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines if a MIDI track has content
        /// </summary>
        /// <param name="bankData">Bank byte data</param>
        /// <param name="patNum">Pattern number (base zero)</param>
        /// <param name="trackNum">MIDI track number (base zero)</param>
        /// <returns>Track number (base zero)</returns>
        public static bool ReadMTrackState(byte[] bankData, int patNum, int trackNum)
        {
            for (int i = 0; i < 8; i++) {
                if (bankData[
                    Constants.ADDR_PAT01 + // Address of pattern 1
                    (Constants.LENGTH_PATTERN_LENGTH * patNum) + // Offset by length of a pattern block * pattern number
                    (Constants.LENGTH_TRAC * 8) + // Skip the tracks to get to the MIDI tracks
                    Constants.LENGTH_PATTERN_HEADER + (Constants.LENGTH_MTRA * trackNum) + Constants.OFFSET_TRACK_TRIGS + i] > 0 )
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Iterates through tracks until it finds something that seems active (not empty)
        /// </summary>
        /// <param name="bankData">Bank byte data</param>
        /// <param name="patNum">Pattern number (base 0)</param>
        /// <returns>True if not empty</returns>
        public static bool ReadPatternActiveState(byte[] bankData, int patNum)
        {
            // TODO: Confirm if reading one byte is enough?
            // I think this may be the trig data in binary format,
            // in which case we would need to read 4 bytes?

            // Iterate through tracks
            for (int t = 0; t < 8; t++)
            {
                if (BankUtils.ReadTrackState(bankData, patNum, t))
                {
                    return true;
                }
            }

            // Iterate through MIDI tracks
            for (int t = 0; t < 8; t++)
            {
                if (BankUtils.ReadMTrackState(bankData, patNum, t))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Reads the part number for the specified pattern
        /// </summary>
        /// <param name="bankData">Bank byte data</param>
        /// <param name="patNum">Patter number (base zero)</param>
        /// <returns>Part number (base zero)</returns>
        public static int ReadPatternPart(byte[] bankData, int patNum)
        {
            return bankData[
                Constants.ADDR_PAT01 + // Address of pattern 1
                (Constants.LENGTH_PATTERN_LENGTH * patNum) + // Offset by length of a pattern block * pattern number
                Constants.OFFSET_PATTERN_PART_NUM];
        }

        /// <summary>
        /// Sawps banks 1 and 2 at the specified path
        /// </summary>
        /// <param name="path">Project path</param>
        /// <param name="bank1">Bank1</param>
        /// <param name="bank2">Bank2</param>
        public static void SwapBanks(string path, int bank1, int bank2)
        {
            string GenerateBankName(int bankNum)
            {
                return path + "\\bank" + bankNum.ToString("00");
            }

            // Temporarily rename bank1 files
            File.Move(GenerateBankName(bank1) + ".work", GenerateBankName(bank1) + ".work.tmp");
            File.Move(GenerateBankName(bank1) + ".strd", GenerateBankName(bank1) + ".strd.tmp");

            // Rename bank2 to bank1
            File.Move(GenerateBankName(bank2) + ".work", GenerateBankName(bank1) + ".work");
            File.Move(GenerateBankName(bank2) + ".strd", GenerateBankName(bank1) + ".strd");

            // Rename bank1 temp to bank2
            File.Move(GenerateBankName(bank1) + ".work.tmp", GenerateBankName(bank2) + ".work");
            File.Move(GenerateBankName(bank1) + ".strd.tmp", GenerateBankName(bank2) + ".strd");

        }
    }
}
