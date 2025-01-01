namespace OctaLib
{
    public class Constants
    {

        // Headers
        public static readonly int[] HEADER_BANK = [
            0x46, 0x4F, 0x52, 0x4D, 0x00, 0x00, 0x00, 0x00,  // FORM
            0x44, 0x50, 0x53, 0x31, 0x42, 0x41, 0x4E, 0x4B]; // DPS1BANK

        // Fixed addresses
        public static readonly int[] ADDR_PART_NAME = [0x0009B4B3, 0x0009B4BA, 0x0009B4C1, 0x0009B4C8];
        public const int ADDR_PAT01 = 0x00000016; // The address of the first pattern header

        // Relative offsets
        public const int OFFSET_TRACK_NUM = 8; // Offset from TRAC/MTRA header

        // See TODO in BankUtils.ReadPatternActiveState
        public const int OFFSET_TRACK_TRIGS = 9; // Offset from TRAC/MTRA header
        public const int OFFSET_TRACK_REC_TRIGS = 41; // Offset from TRAC/MTRA header
        public const int OFFSET_PATTERN_PART_NUM = 0x8EE7; // Offset from the PTRN header

        // Block lengths
        public const int LENGTH_PATTERN_HEADER = 8; // 8 bytes
        public const int LENGTH_PART_NAME = 6; // 6 bytes
        public const int LENGTH_PATTERN_LENGTH = 0x8EEC; // 36588 bytes
        public const int LENGTH_MTRA = 0x8B9; // 2233 bytes
        public const int LENGTH_TRAC = 0x922; // 2338 bytes

        /** 
        Pattern 1 MIDI track addresses: use these to find the start of tracks relative to other pattern headers 
        MTRA01 0x0000492E
        MTRA02 0x000051E7
        MTRA03 0x00005AA0
        MTRA04 0x00006359
        MTRA05 0x00006C12
        MTRA06 0x000074CB
        MTRA07 0x00007D84
        MTRA08 0x0000863D

        Pattern header addresses
        PTRN01 0x00000016
        PTRN02 0x00008F02
        PTRN03 0x00011DEE
        PTRN04 0x0001ACDA
        PTRN05 0x00023BC6
        PTRN06 0x0002CAB2
        PTRN07 0x0003599E
        PTRN08 0x0003E88A
        PTRN09 0x00047776
        PTRN10 0x00050662
        PTRN11 0x0005954E
        PTRN12 0x0006243A
        PTRN13 0x0006B326
        PTRN14 0x00074212
        PTRN15 0x0007D0FE
        PTRN16 0x00085FEA

        There seem to be two sets of parts. Why?
        PART01 0x0008EED6
        PART02 0x00090791
        PART03 0x0009204C
        PART04 0x00093907
        PART05 0x000951C2
        PART06 0x00096A7D
        PART07 0x00098338
        PART08 0x00099BF3
        **/
    }
}
