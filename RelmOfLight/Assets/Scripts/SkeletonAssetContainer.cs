/******************************************************************************
 * Spine Runtimes License Agreement
 * Last updated May 1, 2019. Replaces all prior versions.
 *
 * Copyright (c) 2013-2019, Esoteric Software LLC
 *
 * Integration of the Spine Runtimes into software or otherwise creating
 * derivative works of the Spine Runtimes is permitted under the terms and
 * conditions of Section 2 of the Spine Editor License Agreement:
 * http://esotericsoftware.com/spine-editor-license
 *
 * Otherwise, it is permitted to integrate the Spine Runtimes into software
 * or otherwise create derivative works of the Spine Runtimes (collectively,
 * "Products"), provided that each user of the Products must obtain their own
 * Spine Editor license and redistribution of the Products in any form must
 * include this license and copyright notice.
 *
 * THIS SOFTWARE IS PROVIDED BY ESOTERIC SOFTWARE LLC "AS IS" AND ANY EXPRESS
 * OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN
 * NO EVENT SHALL ESOTERIC SOFTWARE LLC BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES, BUSINESS
 * INTERRUPTION, OR LOSS OF USE, DATA, OR PROFITS) HOWEVER CAUSED AND ON ANY
 * THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
 * EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *****************************************************************************/

using Spine;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


namespace Spine.Unity {
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SkeletonAssetContainer")]
    public class SkeletonAssetContainer : ScriptableObject
    {

        public static SkeletonAssetContainer Instance = null;

        public void Instantiator(SkeletonAssetContainer currentInstance)
        {
            Instance = currentInstance;
        }

        [HideInInspector]public Dictionary<string, SkeletonData> SkeletonDatas = new Dictionary<string, SkeletonData>();

        public List<SkeletonLoaderClass> Jsons = new List<SkeletonLoaderClass>();
        Atlas[] AtlasA = new Atlas[0];
        public void BuildJson(SkeletonLoaderClass slc)
        {
            if(slc.Json != null && slc.AtlasArray != null)
            {

                var input = new MemoryStream(slc.Json.bytes);
               // var input = new StringReader(slc.Json.text);

                AtlasA = new Atlas[slc.AtlasArray.Length];
                for (int i = 0; i < slc.AtlasArray.Length; i++)
                {
                    if (slc.AtlasArray[i] != null)
                    {
                        AtlasA[i] = slc.AtlasArray[i].GetAtlas();
                    }

                }

                var json = new SkeletonBinary(new AtlasAttachmentLoader(AtlasA))
                //var json = new SkeletonJson(new AtlasAttachmentLoader(AtlasA))
                {
                    Scale = slc.SkeletonDataScale
                };
                 //json.ReadSkeletonData(input);

                SkeletonDatas.Add(slc.CharacterName, json.ReadSkeletonData(input));//SkeletonDataAsset.ReadSkeletonData(slc.Json.text, new AtlasAttachmentLoader(slc.AtlasArray), slc.SkeletonDataScale)
            }
        }

        public bool DoesSkeletonExist(string charName)
        {
            return Jsons.Where(r => r.CharacterName == charName).FirstOrDefault() != null;
        }

        public SkeletonData GetSkeletonDataAssetFromName(string charName)
        {
            return SkeletonDatas[charName];
        }
	}
}

[System.Serializable]
public class SkeletonLoaderClass
{
    public string CharacterName;
    public TextAsset Json;
    public AtlasAssetBase[] AtlasArray;
    public float SkeletonDataScale;

    public SkeletonLoaderClass()
    {

    }

    public SkeletonLoaderClass(string characterName, TextAsset json, AtlasAssetBase[] atlasArray, float skeletonDataScale)
    {
        CharacterName = characterName;
        Json = json;
        AtlasArray = atlasArray;
        SkeletonDataScale = skeletonDataScale;
    }
}