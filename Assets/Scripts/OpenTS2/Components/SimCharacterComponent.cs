using System;
using System.Collections.Generic;
using System.Linq;
using OpenTS2.Common;
using OpenTS2.Content;
using OpenTS2.Content.DBPF.Scenegraph;
using OpenTS2.Files.Formats.DBPF;
using UnityEngine;

namespace OpenTS2.Components
{
    /// <summary>
    /// This component represents a rendered out sims character with their head, hair and body meshes in place under one
    /// scenegraph component.
    /// </summary>
    public class SimCharacterComponent : MonoBehaviour
    {
        public static SimCharacterComponent CreateNakedBaseSim()
        {
            const string bodyResourceName = "amBodyMadScientist_cres";
            var bodyAsset = ContentProvider.Get().GetAsset<ScenegraphResourceAsset>(
                new ResourceKey(bodyResourceName, GroupIDs.Scenegraph, TypeIDs.SCENEGRAPH_CRES));

            const string baldHairResourceName = "amHairBald_cres";
            var baldHairAsset = ContentProvider.Get().GetAsset<ScenegraphResourceAsset>(
                new ResourceKey(baldHairResourceName, GroupIDs.Scenegraph, TypeIDs.SCENEGRAPH_CRES));

            const string baseFaceResourceName = "amFace_cres";
            var baseFaceAsset = ContentProvider.Get().GetAsset<ScenegraphResourceAsset>(
                new ResourceKey(baseFaceResourceName, GroupIDs.Scenegraph, TypeIDs.SCENEGRAPH_CRES));

            var simsObject =
                ScenegraphComponent.CreateRootScenegraph(new[] { bodyAsset, baldHairAsset, baseFaceAsset });
            var scenegraph = simsObject.GetComponentInChildren<ScenegraphComponent>();

            var gameObject = new GameObject("sim_character", typeof(SimCharacterComponent));

            //var avatar = AddUnityAvatar(scenegraph);
            //var animator = gameObject.AddComponent<Animator>();
            //animator.avatar = avatar;
            //animator.SetLookAtPosition(new Vector3(20, 0, 0));

            simsObject.transform.parent = gameObject.transform;
            return gameObject.GetComponent<SimCharacterComponent>();
        }

        private static Avatar AddUnityAvatar(ScenegraphComponent scene)
        {
            var humanDescription = new HumanDescription()
            {
                armStretch = 0.05f,
                feetSpacing = 0f,
                hasTranslationDoF = false,
                legStretch = 0.05f,
                lowerArmTwist = 0.5f,
                lowerLegTwist = 0.5f,
                upperArmTwist = 0.5f,
                upperLegTwist = 0.5f,
            };

            var boneNames = new Dictionary<string, string>
            {
                ["Chest"] = "spine1",
                ["UpperChest"] = "spine2",
                ["Head"] = "head",
                ["Neck"] = "neck",
                ["Hips"] = "root_rot",
                ["LeftFoot"] = "l_foot",
                ["LeftHand"] = "l_hand",
                ["LeftLowerArm"] = "l_forearm",
                ["LeftLowerLeg"] = "l_calf",
                ["LeftShoulder"] = "l_clavicle",
                ["LeftUpperArm"] = "l_upperarm",
                ["LeftUpperLeg"] = "l_thigh",
                ["RightFoot"] = "r_foot",
                ["RightHand"] = "r_hand",
                ["RightLowerArm"] = "r_forearm",
                ["RightLowerLeg"] = "r_calf",
                ["RightShoulder"] = "r_clavicle",
                ["RightUpperArm"] = "r_upperarm",
                ["RightUpperLeg"] = "r_thigh",
                ["Spine"] = "spine0",
            };
            var humanBones = new List<HumanBone>();
            foreach (var humanBoneName in HumanTrait.BoneName)
            {
                if (!boneNames.ContainsKey(humanBoneName)) continue;

                var humanBone = new HumanBone
                {
                    humanName = humanBoneName,
                    boneName = boneNames[humanBoneName]
                };
                humanBone.limit.useDefaultValues = true;
                humanBones.Add(humanBone);
            }
            humanDescription.human = humanBones.ToArray();

            var skeleton = new List<SkeletonBone>();
            // hack: need to have root_trans and auskel etc in the skeleton.
            var extraBones = new[] { "root_trans", "auskel", "pelvis", "l_bicep", "r_bicep", "l_wrist", "r_wrist" };
            foreach (var boneName in boneNames.Values.Union(extraBones))
            {
                var boneTransform = scene.BoneNamesToTransform[boneName];
                var bone = new SkeletonBone()
                {
                    name = boneName,
                    position = boneTransform.localPosition,
                    rotation = boneTransform.localRotation,
                    scale = boneTransform.localScale,
                };
                skeleton.Add(bone);
            }
            humanDescription.skeleton = skeleton.ToArray();


            var avatar = AvatarBuilder.BuildHumanAvatar(scene.gameObject, humanDescription);
            avatar.name = scene.name;
            if (!avatar.isValid)
            {
                throw new Exception("Created avatar was not valid");
            }
            return avatar;
        }
    }
}