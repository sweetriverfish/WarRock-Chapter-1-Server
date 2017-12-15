/*
 *              This class allows Cristina to speak to the player using Chat packets
 * 
 * 
 *         PUBLIC METHOD SAY: Allow Cristina to say something to the lobby/room
 *         PUBLICH METHOD WHISPER: Allow Cristina to deliver a message to a certain player
 * 
 * */

using System;
using System.Linq;
using System.Collections.Generic;
using Game.Enums;


namespace Game.Cristina.Modules
{      
    public sealed class Chat
    {
  
        public void SaytoRoom(string _message, Entities.Room Room)
        {
            string _codedMessage = MessageTranslator(_message);
            Room.Send(GetPacket(ChatType.Room_ToAll, _codedMessage));
        }

        public void SayToLobby(string _message)
        {
            string _codedMessage = MessageTranslator(_message);
            Managers.ChannelManager.Instance.SendAllLobbies(GetPacket(ChatType.Lobby_ToAll, _codedMessage));
        }

        public void SayNotice(string _message)
        {
            string _codedMessage = MessageTranslator(_message);
            Managers.ChannelManager.Instance.SendAllLobbies(GetPacket(ChatType.Notice2, _codedMessage));
        }

        public void SayToTeam(string _message, Entities.Room Room, Team Team)
        {
            byte[] buffer = GetPacket(ChatType.Room_ToTeam, MessageTranslator(_message));
            Room.Players.Values.Where(n => n.Team == Team).ToList().ForEach(n => n.Send(buffer));
        }

        public void SayToPlayer(string _message, Entities.User User)
        {
            User.Send(GetWhisperPacket(User, MessageTranslator(_message)));
        }

        //Convert a raw string to a string the game can send
        private string MessageTranslator(string _message)
        {     
            string _codedMessage = _message.Insert(0, ">>" + Convert.ToChar(0x1D));
            _codedMessage = _codedMessage.Replace(Convert.ToChar(0x20), Convert.ToChar(0x1D));

            return _codedMessage;
        }


        //Returns a packet ready to be sent
        private byte[] GetPacket(ChatType _type, string _codedMessage)
        {
            return new Packets.Chat(Core.Cristina.Name, _type, _codedMessage, 999, "NULL").BuildEncrypted();
        }

        //Returns a packet ready to be sent
        private byte[] GetWhisperPacket(Entities.User User, string _codedMessage)
        {
            return new Packets.Chat(Core.Cristina.Name, ChatType.Whisper, _codedMessage, (int)User.SessionID, User.Displayname).BuildEncrypted();
        }


    }
}
