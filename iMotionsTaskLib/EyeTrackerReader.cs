using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iMotionsTaskLib
{
    public class EyeTrackerReader : IEyeTrackerReader
    {
        public EyeTrackerReader(IEyeTrackerData data)
        {
            this.data = data;
            cIndex.Clear();
        }

        private IEyeTrackerData data;
        private UTF8Encoding encoding = new UTF8Encoding(true);

        private long timeOffset = 0;
        private bool firstRecord = true;

        struct ColumnIndexes
        {
            public int time;

            public int leftX;
            public int leftY;
            public int rightX;
            public int rightY;

            public int maxIndex;

            public void Clear()
            {
                time = -1;
                leftX = -1;
                leftY = -1;
                rightX = -1;
                rightY = -1;
                maxIndex = -1;
            }

            public bool IsValid()
            {
                return time != -1 &&
                       leftX != -1 &&
                       leftY != -1 &&
                       rightX != -1 &&
                       rightY != -1;
            }

            public void updateMaxIndex()
            {
                maxIndex = Math.Max(time, Math.Max(leftX, Math.Max(leftY, Math.Max(rightX, rightY))));
            }
        }
        ColumnIndexes cIndex;

        //private string filePath = "";
        private string filePath = ".\\Dump001_M1Resp_52.txt";//@"C:\dev\iMotionsTask\iMotionsTask\Dump001_M1Resp_52.txt";
        public string FilePath
        {
            get { return filePath; }
            set
            {
                if (filePath != value)
                {
                    filePath = value;
                }
            }
        }
        

        public bool ParseFile()
        {
            if (data == null)
            {
                // Todo: Report proper error
                return false;
            }

            if (string.IsNullOrEmpty(filePath))
            {
                // Todo: Report proper error
                return false;
            }

            if (!File.Exists(filePath))
            {
                // Todo: Report proper error
                return false;
            }

            cIndex.Clear();
            timeOffset = 0;
            firstRecord = true;

            using (StreamReader sr = new StreamReader(FilePath))
            {
                string line;
                // Search for and parse the header
                while ((line = sr.ReadLine()) != null)
                {
                    if (parseHeaderLine(line))
                    {
                        break;
                    }
                }

                // Only continues if all data columns have been located in the header
                if (!cIndex.IsValid()) { return false; }

                // Pase and register data
                while ((line = sr.ReadLine()) != null)
                {
                    parseRecordLine(line);
                }
            }
            return true;
        }

        // Returns true when header have been parsed
        private bool parseHeaderLine(string line)
        {
            // Other info starts with #
            if (!line.Any() || line[0] == '#')
            {
                // Skip other info for now
                return false;
            }

            // Skip empty lines
            if (line[0] == ' ') { return false; }

            // First line is the header
            string[] headerList = line.Split('\t');
            for (int i = 0; i < headerList.Count(); i++)
            {
                switch (headerList[i])
                {
                    case "Timestamp":
                        cIndex.time = i;
                        break;
                    case "GazeLeftx":
                        cIndex.leftX = i;
                        break;
                    case "GazeLefty":
                        cIndex.leftY = i;
                        break;
                    case "GazeRightx":
                        cIndex.rightX = i;
                        break;
                    case "GazeRighty":
                        cIndex.rightY = i;
                        break;
                    default:
                        break;
                }
            }
            cIndex.updateMaxIndex();
            return true;
        }

        private void parseRecordLine(string line)
        {
            int columnIndex = 0;
            int byteIndexBegin = 0;

            EyeTrackerRecord record;

            // Needed for the compiler to figure out all element have been assigned...
            record.t = 0;
            record.leftX = 0;
            record.leftY = 0;
            record.rightX = 0;
            record.rightY = 0;

            for (int byteIndexEnd = 0; byteIndexEnd < line.Length; )
            {
                // Only parse data if byteIndexEnd found a tab
                if (line[byteIndexEnd] != '\t') {
                    byteIndexEnd++;
                    // Or if end of line is reached
                    if (byteIndexEnd != line.Length)
                    {
                        continue;
                    }
                }

                // Parse the data if column index is used
                if (columnIndex == cIndex.time)
                {
                    var str = line.Substring(byteIndexBegin, byteIndexEnd - byteIndexBegin);
                    if (!long.TryParse(str, out record.t)) { return; }
                    if (record.t < 0) { return; }
                    if (firstRecord) {
                        firstRecord = false;
                        timeOffset = record.t;
                    }
                    record.t -= timeOffset;
                }
                else if (columnIndex == cIndex.leftX)
                {
                    var str = line.Substring(byteIndexBegin, byteIndexEnd - byteIndexBegin);
                    if (!int.TryParse(str, out record.leftX)) { return; }
                    if (record.leftX < 0) { return; }
                }
                else if (columnIndex == cIndex.leftY)
                {
                    var str = line.Substring(byteIndexBegin, byteIndexEnd - byteIndexBegin);
                    if (!int.TryParse(str, out record.leftY)) { return; }
                    if (record.leftY < 0) { return; }
                }
                else if (columnIndex == cIndex.rightX)
                {
                    var str = line.Substring(byteIndexBegin, byteIndexEnd - byteIndexBegin);
                    if (!int.TryParse(str, out record.rightX)) { return; }
                    if (record.rightX < 0) { return; }
                }
                else if (columnIndex == cIndex.rightY)
                {
                    var str = line.Substring(byteIndexBegin, byteIndexEnd - byteIndexBegin);
                    if (!int.TryParse(str, out record.rightY)) { return; }
                    if (record.rightY < 0) { return; }
                }

                byteIndexEnd++;
                byteIndexBegin = byteIndexEnd;
                columnIndex++;
            }

            // Make sure all needed data have been parsed
            if (columnIndex > cIndex.maxIndex)
            {
                data.Add(record);
            }
        }
    }
}
