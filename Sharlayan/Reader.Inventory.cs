﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Reader.Inventory.cs" company="SyndicatedLife">
//   Copyright© 2007 - 2021 Ryan Wilson &amp;lt;syndicated.life@gmail.com&amp;gt; (https://syndicated.life/)
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   Reader.Inventory.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sharlayan {
    using System;

    using Sharlayan.Core;
    using Sharlayan.Core.Enums;
    using Sharlayan.Models.ReadResults;

    public partial class Reader {
        public bool CanGetInventory() {
            bool canRead = this._memoryHandler.Scanner.Locations.ContainsKey(Signatures.InventoryKey);
            if (canRead) {
                // OTHER STUFF?
            }

            return canRead;
        }

        public InventoryResult GetInventory() {
            InventoryResult result = new InventoryResult();

            if (!this.CanGetInventory() || !this._memoryHandler.IsAttached) {
                return result;
            }

            try {
                const int inventoryCount = 74;
                const int inventoryByteCount = 24;

                IntPtr inventoryMap = new IntPtr(this._memoryHandler.GetInt64(this._memoryHandler.Scanner.Locations[Signatures.InventoryKey]));
                byte[] inventoryBytes = this._memoryHandler.GetByteArray(inventoryMap, inventoryCount * 24);

                for (int i = 0; i < inventoryCount; i++) {
                    int bagIndex = i * inventoryByteCount;
                    uint bagID = BitConverter.ToUInt32(inventoryBytes, bagIndex + this._memoryHandler.Structures.InventoryContainer.ID);

                    if (!Enum.IsDefined(typeof(Inventory.Container), bagID)) {
                        continue;
                    }

                    InventoryContainer container = new InventoryContainer {
                        Amount = BitConverter.ToUInt32(inventoryBytes, bagIndex + this._memoryHandler.Structures.InventoryContainer.Amount),
                        TypeID = bagID,
                        ContainerType = (Inventory.Container) bagID,
                    };

                    int itemByteCount = 56;

                    IntPtr slotMap = new IntPtr(this._memoryHandler.GetInt64FromBytes(inventoryBytes, bagIndex));
                    byte[] slotBytes = this._memoryHandler.GetByteArray(slotMap, (int) container.Amount * itemByteCount);

                    for (int j = 0; j < container.Amount; j++) {
                        int slotIndex = j * itemByteCount;
                        uint itemId = BitConverter.ToUInt32(slotBytes, slotIndex + this._memoryHandler.Structures.InventoryItem.ID);

                        if (itemId <= 0) {
                            container.InventoryItems.Add(new InventoryItem());
                            continue;
                        }

                        container.InventoryItems.Add(
                            new InventoryItem {
                                Slot = BitConverter.ToUInt16(slotBytes, slotIndex + this._memoryHandler.Structures.InventoryItem.Slot),
                                ID = itemId,
                                Amount = BitConverter.ToUInt32(slotBytes, slotIndex + this._memoryHandler.Structures.InventoryItem.Amount),
                                SB = BitConverter.ToUInt16(slotBytes, slotIndex + this._memoryHandler.Structures.InventoryItem.SB),
                                Condition = BitConverter.ToUInt16(slotBytes, slotIndex + this._memoryHandler.Structures.InventoryItem.Durability),
                                IsHQ = (slotBytes[slotIndex + this._memoryHandler.Structures.InventoryItem.IsHQ] & 1) == 1,
                                MateriaTypes = new[] {
                                    (Inventory.MateriaType) slotBytes[slotIndex + this._memoryHandler.Structures.InventoryItem.MateriaType],
                                    (Inventory.MateriaType) slotBytes[slotIndex + 2 + this._memoryHandler.Structures.InventoryItem.MateriaType],
                                    (Inventory.MateriaType) slotBytes[slotIndex + 4 + this._memoryHandler.Structures.InventoryItem.MateriaType],
                                    (Inventory.MateriaType) slotBytes[slotIndex + 6 + this._memoryHandler.Structures.InventoryItem.MateriaType],
                                    (Inventory.MateriaType) slotBytes[slotIndex + 8 + this._memoryHandler.Structures.InventoryItem.MateriaType],
                                },
                                MateriaRanks = new[] {
                                    slotBytes[slotIndex + this._memoryHandler.Structures.InventoryItem.MateriaRank],
                                    slotBytes[slotIndex + 1 + this._memoryHandler.Structures.InventoryItem.MateriaRank],
                                    slotBytes[slotIndex + 2 + this._memoryHandler.Structures.InventoryItem.MateriaRank],
                                    slotBytes[slotIndex + 3 + this._memoryHandler.Structures.InventoryItem.MateriaRank],
                                    slotBytes[slotIndex + 4 + this._memoryHandler.Structures.InventoryItem.MateriaRank],
                                },
                                DyeID = slotBytes[slotIndex + this._memoryHandler.Structures.InventoryItem.DyeID],
                                GlamourID = BitConverter.ToUInt32(slotBytes, slotIndex + this._memoryHandler.Structures.InventoryItem.GlamourID),
                            });
                    }

                    result.InventoryContainers.Add(container);
                }
            }
            catch (Exception ex) {
                this._memoryHandler.RaiseException(Logger, ex, true);
            }

            return result;
        }
    }
}