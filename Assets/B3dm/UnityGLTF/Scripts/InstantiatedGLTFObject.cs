using Assets;
using DotSpatial.Positioning;
using Terrain;
using UnityEngine;
using UnityGLTF.Cache;

namespace UnityGLTF
{
    /// <summary>
    /// Instantiated GLTF Object component that gets added to the root of every GLTF game object created by a scene importer.
    /// </summary>
    public class InstantiatedGLTFObject : MonoBehaviour
    {
        bool drawEdges = false;
        bool drawVertexColors = false;
        bool replaceWithStandard = true;
        /// <summary>
        /// Ref-counted cache data for this object.
        /// The same instance of this cached data will be used for all copies of this GLTF object,
        /// and the data gets cleaned up when the ref counts goes to 0.
        /// </summary>
        private RefCountedCacheData cachedData;
        public RefCountedCacheData CachedData
        {
            get
            {
                return cachedData;
            }

            set
            {
                if (cachedData != value)
                {
                    if (cachedData != null)
                    {
                        cachedData.DecreaseRefCount();
                    }

                    cachedData = value;

                    if (cachedData != null)
                    {
                        cachedData.IncreaseRefCount();
                    }
                }
            }
        }

        /// <summary>
        /// Duplicates the instantiated GLTF object.
        /// Note that this should always be called if you intend to create a new instance of a GLTF object, 
        /// in order to properly preserve the ref count of the dynamically loaded mesh data, otherwise
        /// you will run into a memory leak due to non-destroyed meshes, textures and materials.
        /// </summary>
        /// <returns></returns>
        public InstantiatedGLTFObject Duplicate()
        {
            GameObject duplicatedObject = Instantiate(gameObject);

            InstantiatedGLTFObject newGltfObjectComponent = duplicatedObject.GetComponent<InstantiatedGLTFObject>();
            newGltfObjectComponent.CachedData = CachedData;
			CachedData.IncreaseRefCount();

            return newGltfObjectComponent;
        }

        private void OnDestroy()
        {
            CachedData = null;
        }

        private void Start()
        {
            //var mesh = GetComponentInChildren<MeshFilter>().sharedMesh;

            //var verts = new Vector3[mesh.vertices.Length];
            //for (int i = 0; i < verts.Length; i++)
            //{
                
            //    var dist_x = new Distance(mesh.vertices[i].x, DistanceUnit.Meters); //lon
            //    var dist_y = new Distance(mesh.vertices[i].y, DistanceUnit.Meters); //lat
            //    var dist_z = new Distance(mesh.vertices[i].z, DistanceUnit.Meters); //alt

            //    var WGS84Coor = new CartesianPoint(dist_x, dist_y, dist_z).ToPosition3D();

            //    double x = WGS84Coor.Longitude.DecimalDegrees;
            //    double height = WGS84Coor.Altitude.Value;
            //    double z = WGS84Coor.Latitude.DecimalDegrees;

            //    //conversion to view
            //    var xv = (x - MapViewer.floatingOrigin.x) * MapViewer.UnityUnitsPerGraadX;
            //    var zv = (z - MapViewer.floatingOrigin.y) * MapViewer.UnityUnitsPerGraadY;

            //    verts[i] = new Vector3((float)xv, (float)height, (float)zv);
            //}
            //mesh.vertices = verts;


            if (drawVertexColors)
            {
                var batchIds = CachedData.MeshCache[0][0].MeshAttributes["_BATCHID"].AccessorContent.AsUInts;

                Color[] colors = new Color[batchIds.Length];
                for (int i = 0; i < batchIds.Length; i++)
                {
                    colors[i] = ColorLookupTable.colors[(int)batchIds[i]];
                }

                var m = GetComponentInChildren<MeshFilter>().sharedMesh;
                m.colors = colors;

                var renderer = GetComponentInChildren<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Custom/StandardShaderWithVertexColor"));
            }

            if (drawEdges)
                GetComponentInChildren<MeshFilter>().gameObject.AddComponent<Wireframe>();

            if (replaceWithStandard)
            {
                var renderer = GetComponentInChildren<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
            }
        }

        private void OnMouseDown()
        {
           
        }
    }
}
