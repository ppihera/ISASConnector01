using System.Collections.Generic;

namespace IsasConnectorBase
{
    public class OutputDTO
    {
        public InputDTO Input { get; set; }
        public List<RecordDTO> Records { get; set; } = new List<RecordDTO>();

        public OutputDTO(InputDTO input)
        {
            Input = input;
        }
    }

    public class RecordDTO
    {
        public int Index { get; set; }
        public List<FieldDTR> Fields { get; set; } = new List<FieldDTR>();
    }

    public class FieldDTR { public string Key; public string Value; }
}
