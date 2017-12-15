using System;

namespace Game.Objects
{
    public sealed class Clan
    {
        public string Name { get; private set; }
        public uint Id { get; private set; }
        public string Tag { get; private set; }
        public uint Members { get; private set; }
        public uint Victories { get; private set; }
        public uint Defeats { get; private set; }
        public uint Draws { get; private set; }
        public uint Master { get; private set; }
        public uint TotalMatches { get { return (Victories + Defeats + Draws); } }

        public Clan(string _name, string _tag, uint _id, uint _members, uint _victories, uint _defeats, uint _draws, uint _master)
        {
            Name = _name;
            Tag = _tag;
            Id = _id;
            Members = _members;
            Victories = _victories;
            Defeats = _defeats;
            Master = _master;
            Draws = _draws;
        }

        public void ResetMatches()
        {
            Victories = 0;
            Defeats = 0;
            Draws = 0;
        }

        public void ChangeMaster(uint _newMasterId)
        {
            Master = _newMasterId;
        }

        public void SetName(string _newName)
        {
            if (Name != _newName)
                Name = _newName;
        }

        public void SetTag(string _newTag)
        {
            if (Tag != _newTag)
                Tag = _newTag;
        }
    }
}
