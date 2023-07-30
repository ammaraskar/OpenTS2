using System.Linq;
using OpenTS2.Common;
using OpenTS2.Components;
using OpenTS2.Files.Formats.DBPF;
using OpenTS2.Files.Formats.DBPF.Scenegraph;
using OpenTS2.Files.Formats.DBPF.Scenegraph.Block;
using UnityEngine;

namespace OpenTS2.Content.DBPF.Scenegraph
{
    public class ScenegraphResourceAsset : AbstractAsset
    {
        public ScenegraphResourceCollection ResourceCollection { get; }

        public ScenegraphResourceAsset(ScenegraphResourceCollection resourceCollection) =>
            (ResourceCollection) = (resourceCollection);


        /// <summary>
        /// Creates a unity game object rendering this scenegraph resource.
        /// </summary>
        public GameObject CreateGameObjectForShape()
        {
            var firstResourceNode = ResourceCollection.Blocks.OfType<ResourceNodeBlock>().First();
            var resourceName = firstResourceNode.ResourceName;

            // Temporary hack until I figure out what has multiple shapes...
            if (ResourceCollection.Blocks.OfType<ShapeRefNodeBlock>().Count() != 1)
            {
                return null;
            }
            var shapeRef = ResourceCollection.GetBlockOfType<ShapeRefNodeBlock>();
            var shapeTransform = shapeRef.Renderable.Bounded.Transform;

            // TODO: handle multiple shapes here.
            var shapeKey = ResourceCollection.FileLinks[shapeRef.Shapes[0].Index];
            if (shapeKey.GroupID == GroupIDs.Local)
            {
                // Use our groupId if the reference has a local group id.
                shapeKey = shapeKey.WithGroupID(GlobalTGI.GroupID);
            }
            var shape = ContentProvider.Get().GetAsset<ScenegraphShapeAsset>(shapeKey);

            shape.LoadModelsAndMaterials();

            var gameObject = new GameObject(resourceName, typeof(AssetReferenceComponent));

            // Apply a transformation to convert from the sims coordinate space to unity.
            gameObject.transform.Rotate(-90, 0, 0);
            gameObject.transform.localScale = new Vector3(1, -1, 1);

            // Keeps a strong reference to the Shape asset.
            gameObject.GetComponent<AssetReferenceComponent>().AddReference(shape);

            // This is the component that holds rotations from sims space. All rotations from the game such as applying
            // quaternions and angles should be performed on it or components under it.
            var simsRotation = new GameObject("simsRotations");

            // Render out each model.
            foreach (var model in shape.Models)
            {
                foreach (var primitive in model.Primitives)
                {
                    // Create an object for the primitive and parent it to the root game object.
                    var primitiveObject = new GameObject($"{resourceName}_{primitive.Key}", typeof(MeshFilter), typeof(MeshRenderer))
                        {
                            transform =
                            {
                                rotation = shapeTransform.Rotation,
                                position = shapeTransform.Transform
                            }
                        };

                    primitiveObject.GetComponent<MeshFilter>().mesh = primitive.Value;
                    if (shape.Materials.TryGetValue(primitive.Key, out var material))
                    {
                        primitiveObject.GetComponent<MeshRenderer>().material = material.GetAsUnityMaterial();
                    }

                    primitiveObject.transform.SetParent(simsRotation.transform);
                }
            }

            // Go through any other cTransformNodes as these might have references to other resource assets.
            foreach (var transform in ResourceCollection.Blocks.OfType<TransformNodeBlock>())
            {
                foreach (var reference in transform.CompositionTree.References)
                {
                    if (reference.Index == -1)
                    {
                        continue;
                    }

                    var nthBlock = ResourceCollection.Blocks[reference.Index];

                    var rnode = (ResourceNodeBlock)nthBlock;

                    var location = rnode.ResourceLocation;
                    var key = ResourceCollection.FileLinks[location.Index];
                    Debug.Assert(key.TypeID == TypeIDs.SCENEGRAPH_CRES);
                    var resource = ContentProvider.Get().GetAsset<ScenegraphResourceAsset>(key);

                    var subObject = resource.CreateGameObjectForShape();
                    if (subObject == null)
                    {
                        continue;
                    }

                    var newTransform = new Vector3(transform.Transform.x, transform.Transform.z, transform.Transform.y);
                    subObject.transform.Find("simsRotations").position = newTransform;
                    subObject.transform.Find("simsRotations").localRotation = transform.Rotation;
                    subObject.transform.SetParent(gameObject.transform, true);
                }
            }

            simsRotation.transform.SetParent(gameObject.transform, worldPositionStays:false);
            return gameObject;
        }
    }
}