using System;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using XiheFramework.Core.Entity;

namespace AutoAddressable {
    public static class AutoAddressableHelper {
        public static bool MarkAddressable(string folderName, string assetPath, out string address) {
            address = "";
            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            var entity = AssetDatabase.LoadAssetAtPath<GameEntity>(AssetDatabase.GUIDToAssetPath(guid));
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (entity == null || string.IsNullOrEmpty(entity.EntityName)) {
                settings.RemoveAssetEntry(guid);
                return false;
            }

            string groupName = folderName;

            AddressableAssetGroup group = settings.FindGroup(groupName);
            if (group == null) {
                group = settings.CreateGroup(groupName, false, false, true, null);
                Debug.Log("[Auto Addressable]Created Addressable Group: " + groupName);
            }

            string assetAddress = entity.EntityGroupName + "_" + entity.EntityName;
            AddressableAssetEntry entry = settings.CreateOrMoveEntry(guid, group);
            entry.address = assetAddress;
            address = assetAddress;
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, null, true);
            return true;
        }
    }
}