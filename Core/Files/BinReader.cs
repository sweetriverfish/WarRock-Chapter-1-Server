using System;
using System.IO;

using System.Collections;

using Core.Files.Bin;

namespace Core.Files {
    public class BinReader : BinHandler {
        private string strFileLocation;
        private string strDecryptedContent;
        private int intLines;

        public TypedObject Data { get; private set; } // CONTAINS ALL THE INFO

        public BinReader(string strFileLocation)
            : base() {
            this.strFileLocation = strFileLocation;
        }

        public bool Read() {
            StreamReader srReader = null;
            try {
                srReader = new StreamReader(File.Open(strFileLocation, FileMode.Open, FileAccess.Read));
                strDecryptedContent = this.DecryptContent(srReader.ReadToEnd());
                srReader.Close();
                this.ParseContent();
                return true;
            } catch {
                //Log.WriteError(ex.ToString());
            } finally {
                try {
                    if (srReader != null)
                        srReader.Close();
                } catch { }
            }

            return false;
        }

        private void ParseContent() {

            Data = new TypedObject();
            string[] strLines = strDecryptedContent.Split(new string[] { "\n" }, StringSplitOptions.None);
            this.intLines = strLines.Length;

            ArrayList arrTabDepth = new ArrayList();
            ArrayList arrBlocks = new ArrayList();
            ArrayList arrSections = new ArrayList();
            TypedObject htDataGroup = new TypedObject();

            int intKeyID = 0;

            // Loop trough each line.
            for (int i = 0; i < strLines.Length; i++) {
                string strCurrentLine = strLines[i];
                string strBegin = strLines[i].Split('=')[0];
                int intTabCount = strBegin.Length - strBegin.Replace("\t", "").Length;

                // Tab count is depth
                int intDirection = 0;
                if (arrTabDepth.Count != intTabCount) {
                    intDirection = intTabCount - arrTabDepth.Count;
                }

                // Remove Tabs
                string strValue = strBegin.Replace("\t", "").Trim();
                // Check type
                if (strValue.StartsWith("[") && strValue.EndsWith("]")) {
                    string strArrayKey = strValue.Replace("[", "").Replace("]", "").ToLower(); // Remove brachets

                    if (!strArrayKey.StartsWith("/")) {
                        // ADD BLOCKS //
                        if (arrTabDepth.Count <= intTabCount)
                            arrTabDepth.Add(strArrayKey);
                        else
                            arrTabDepth[arrTabDepth.Count - 1] = strArrayKey;

                        arrBlocks.Add(new TypedObject());
                    } else {
                        // End of the block, Save it
                        int intCurrentKeyIndex = arrTabDepth.Count - 1;
                        string strCurrentKey = arrTabDepth[intCurrentKeyIndex].ToString();
                        int intCurrentBlock = arrBlocks.Count - 1;


                        // Copy it all over, lazy way.
                        TypedObject htCopy = new TypedObject(htDataGroup.Type);
                        if (intKeyID > 0) {
                            foreach (DictionaryEntry entry in htDataGroup) {
                                if (htCopy.ContainsKey(entry.Key))
                                    htCopy[entry.Key] = entry.Value;
                                else
                                    htCopy.Add(entry.Key, entry.Value);
                            }
                            htDataGroup.Clear();
                            intKeyID = 0;
                        } else {
                            foreach (DictionaryEntry entry in (TypedObject)arrBlocks[intCurrentBlock]) {
                                if (htCopy.ContainsKey(entry.Key))
                                    htCopy[entry.Key] = entry.Value;
                                else
                                    htCopy.Add(entry.Key, entry.Value);
                            }
                        }

                        // Remove Hashtable //

                        arrBlocks.RemoveAt(intCurrentBlock);

                        if (arrBlocks.Count > 0)
                            ((TypedObject)arrBlocks[arrBlocks.Count - 1]).Add(strCurrentKey, htCopy); // move to previous block
                        else
                            Data.Add(strCurrentKey, htCopy);
                        arrTabDepth.RemoveAt(intCurrentKeyIndex); // Remove from index.
                    }
                } else if (strValue.StartsWith("<") && !strValue.StartsWith("<!") && strValue.EndsWith(">")) {
                    // Sub section we go one deeper!
                    string strSectionKey = strValue.Replace("<", "").Replace(">", "").ToLower(); // Remove <>

                    if (!strSectionKey.StartsWith("/")) // NEW SECTION //
                    {
                        if (arrTabDepth.Count <= intTabCount)
                            arrTabDepth.Add(strSectionKey);
                        else
                            arrTabDepth[arrTabDepth.Count - 1] = strSectionKey;

                        //Console.WriteLine("<" + strSectionKey.ToUpper() + "> @ Line: " + (i + 1).ToString() + " ADD SECTION");

                        arrSections.Add(new TypedObject());
                    } else {
                        // REMOVE SECTION //
                        int intSectionIndex = arrTabDepth.Count - 1;
                        string strCurrentKey = arrTabDepth[intSectionIndex].ToString();

                        int intCurrentSubSection = arrSections.Count - 1;

                        // Copy it all over, lazy way.
                        TypedObject htCopy = new TypedObject();
                        foreach (DictionaryEntry entry in (TypedObject)(arrSections[intCurrentSubSection])) {
                            if (htCopy.ContainsKey(entry.Key))
                                htCopy[entry.Key] = entry.Value;
                            else
                                htCopy.Add(entry.Key, entry.Value);
                        }

                        // Remove Hashtable of the current section //
                        arrSections.RemoveAt(intCurrentSubSection);

                        //Console.WriteLine("<" + strSectionKey.ToUpper() + "> @ Line: " + (i + 1).ToString() + " REMOVE SECTION");

                        // Are there sub sections underneath?
                        if (arrSections.Count > 0) { // Yes, Let's insert the data in the previous one.
                            // Check for duplicates.
                            if (((TypedObject)arrSections[arrSections.Count - 1]).ContainsKey(strCurrentKey)) {
                                int intNumber = 1;
                                string newKey = string.Concat(strCurrentKey, intNumber);
                                while (((TypedObject)arrSections[arrSections.Count - 1]).ContainsKey(newKey)) {
                                    intNumber++;
                                    newKey = string.Concat(strCurrentKey, i);
                                }

                                ((TypedObject)arrSections[arrSections.Count - 1]).Add(newKey, htCopy);
                            } else {
                                ((TypedObject)arrSections[arrSections.Count - 1]).Add(strCurrentKey, htCopy); // Add it
                            }
                        } else { // No, okay.. Add the data to the branch.
                            ((TypedObject)arrBlocks[arrBlocks.Count - 1]).Add(strCurrentKey, htCopy);
                        }

                        arrTabDepth.RemoveAt(intSectionIndex); // Remove section
                    }
                } else {
                    if (arrTabDepth.Count > 1) {
                        string strLine = strCurrentLine.Replace("\t", "").Replace(" ", ""); // Remove tabs + spaces
                        if (strLine.StartsWith("<!")) {
                            // New group of data.
                            intKeyID = intKeyID + 1; // Increase key ID
                        } else if (strLine.StartsWith("//") && strLine.EndsWith("->")) {
                            // Copy it all over, lazy way.
                            TypedObject htCopy = new TypedObject();
                            foreach (DictionaryEntry entry in (TypedObject)arrBlocks[arrBlocks.Count - 1]) {
                                if (htCopy.ContainsKey(entry.Key))
                                    htCopy[entry.Key] = entry.Value;
                                else
                                    htCopy.Add(entry.Key, entry.Value);
                            }

                            htDataGroup.Add(intKeyID, htCopy);
                            ((TypedObject)arrBlocks[arrBlocks.Count - 1]).Clear();
                        } else {
                            string[] sectionData = strLine.Split('=');
                            string key = sectionData[0]; string value = "";

                            if (sectionData.Length > 1) // Just in case
                                value = sectionData[1];

                            ((TypedObject)arrSections[arrSections.Count - 1]).Add(key.ToLower(), value);
                        }

                    } else {
                        // Unknown line.
                    }
                }
            }

        }

        private string DecryptContent(byte[] bBuffer) {
            return this.DecryptContent(System.Text.Encoding.UTF8.GetString(bBuffer));
        }

        private string DecryptContent(string strInput) {
            for (int i = 0; i <= strInput.Length - 2; i++) {
                if ((i + 2) % 2 == 0) {
                    lstbOutputBuffer.Add(byte.Parse(this.htDecryptBuffer[strInput[i].ToString() + strInput[i + 1].ToString()].ToString()));
                }
            }
            return System.Text.Encoding.UTF8.GetString(lstbOutputBuffer.ToArray());
        }
    }
}
