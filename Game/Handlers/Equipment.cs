using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Game.Enums;

namespace Game.Handlers {
    class Equipment : Networking.PacketHandler {
        protected override void Process(Entities.User u) {
            if (u.Authorized) {
                bool equipItem = !GetBool(0);
                byte bTargetClass = GetByte(1);
                if (bTargetClass < Objects.Inventory.Equipment.MAX_CLASSES)
                {
                    Classes targetClass = (Classes)bTargetClass;
                    string weaponCode = GetString(4).ToUpper();
                    byte targetSlot = 0;
                    if (weaponCode.Length == 4)
                    {
                        if (targetSlot < 8)
                        {
                            if (equipItem)
                            {
                                targetSlot = GetByte(5);
                                if (targetSlot < Objects.Inventory.Equipment.MAX_SLOTS)
                                {
                                    if (Managers.ItemManager.Instance.Items.ContainsKey(weaponCode))
                                    {
                                        Objects.Items.ItemData item = null;
                                        if (Managers.ItemManager.Instance.Items.TryGetValue(weaponCode, out item))
                                        {
                                            if (item.IsWeapon)
                                            {
                                                Objects.Items.Weapon weapon = (Objects.Items.Weapon)item;
                                                if (weapon != null)
                                                {
                                                    if ((weapon.Active && weapon.CanEquip[(byte)targetClass, targetSlot]) || u.AccessLevel > 3)
                                                    {
                                                        Objects.Inventory.Item equipmentItem = u.Inventory.Get(weapon.Code);
                                                        if (equipmentItem != null && equipmentItem.Slot >= 0)
                                                        { // Does the user have the item.
                                                            Objects.Inventory.Item equipedItem = u.Inventory.Equipment.Get(targetClass, targetSlot);
                                                            if (!u.Inventory.Retails.Contains(weapon.Code.ToUpper()) && (equipedItem == null || equipmentItem.Slot != equipedItem.Slot))
                                                            {
                                                                // string Type = getBlock(2);
                                                                if (equipmentItem.Equiped[(byte)targetClass] >= 0)
                                                                    u.Inventory.Equipment.Remove(targetClass, (byte)equipmentItem.Equiped[(byte)targetClass]);

                                                                u.Inventory.Equipment.Add(targetClass, targetSlot, equipmentItem);
                                                                u.Inventory.Equipment.Build();
                                                                u.Inventory.Equipment.BuildInternal();
                                                                u.Send(new Packets.Equipment(targetClass, u.Inventory.Equipment.ListsInternal[(byte)targetClass]));
                                                            }
                                                            else
                                                            {
                                                                u.Send(new Packets.Equipment(Packets.Equipment.ErrorCode.AlreadyEquipped)); // Already equiped.
                                                            }
                                                        }
                                                        else
                                                        {
                                                            bool isFound = false; // ATTAMPT TO CHECK IF THE ITEM IS A DEFAULT ITEM.
                                                            Objects.Inventory.Item equipedItem = u.Inventory.Equipment.Get(targetClass, targetSlot);
                                                            for (byte j = 0; j < Objects.Inventory.Inventory.DEFAULT_ITEMS.Length; j++)
                                                            {
                                                                if (weaponCode == Objects.Inventory.Inventory.DEFAULT_ITEMS[j])
                                                                {
                                                                    isFound = true;
                                                                    if (equipedItem == null || equipedItem.Slot != -1)
                                                                    {
                                                                        u.Inventory.Equipment.Add(targetClass, targetSlot, new Objects.Inventory.Item(-1, 0, Objects.Inventory.Inventory.DEFAULT_ITEMS[j], 0));
                                                                        u.Inventory.Equipment.Build();
                                                                        u.Inventory.Equipment.BuildInternal();
                                                                        u.Send(new Packets.Equipment(targetClass, u.Inventory.Equipment.ListsInternal[(byte)targetClass]));
                                                                    }
                                                                    else
                                                                    {
                                                                        u.Send(new Packets.Equipment(Packets.Equipment.ErrorCode.AlreadyEquipped)); // Already equiped.
                                                                    }
                                                                    break;
                                                                }
                                                            }
                                                            if (!isFound)
                                                            {
                                                                u.Disconnect(); // potentiality scripting or packet changing.. TODO: LOG
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        u.Disconnect(); // potentiality scripting or packet changing.. TODO: LOG
                                                    }
                                                }
                                                else
                                                {
                                                    u.Disconnect(); // Server error?
                                                }
                                            }
                                        }
                                        else
                                        {
                                            u.Disconnect(); // Server error?
                                        }
                                    }
                                    else
                                    {
                                        u.Disconnect(); // potentiality scripting or packet changing.. TODO: LOG
                                    }

                                }
                                else
                                {
                                    u.Disconnect(); // potentiality scripting or packet changing.. TODO: LOG
                                }
                            }
                            else
                            {
                                targetSlot = GetByte(3);
                                Objects.Inventory.Item equipedItem = u.Inventory.Equipment.Get(targetClass, targetSlot);
                                if (equipedItem != null)
                                {
                                    u.Inventory.Equipment.Remove(targetClass, targetSlot);
                                    u.Inventory.Equipment.Build();
                                    u.Send(new Packets.Equipment(targetClass, u.Inventory.Equipment.Lists[(byte)targetClass]));
                                }
                            }
                        }
                    }
                }
                else
                {
                    u.Disconnect();
                }
            }
        }
    }
}
