using System;
using System.Collections;
using System.Collections.Generic;
using Mapbox;
using Mapbox.Map;
using UnityEngine;
using UnityEngine.UI;
using Mapbox.MeshGeneration.Enums;
using Mapbox.Scripts.Utilities;
using Mapbox.MeshGeneration.Data;

namespace Mapbox.MeshGeneration.Factories
{
    [CreateAssetMenu(menuName = "Mapbox/Factories/Map Image Factory")]
    public class MapImageFactory : Factory
    {
        [SerializeField]
        public string _mapId;
        [SerializeField]
        public Material _baseMaterial;
        [SerializeField]
        private bool _createImagery = true;
		[SerializeField]
		public bool _isLayer = false;

        private Dictionary<Vector2, UnityTile> _tiles;

        public override void Initialize(MonoBehaviour mb, IFileSource fs)
        {
            base.Initialize(mb, fs);
            _tiles = new Dictionary<Vector2, UnityTile>();
        }

        public override void Register(UnityTile tile)
        {
            base.Register(tile);
            _tiles.Add(tile.TileCoordinate, tile);
            Run(tile);
        }

        private void Run(UnityTile tile)
        {
            if (!string.IsNullOrEmpty(_mapId))
            {
                var parameters = new Tile.Parameters();
                parameters.Fs = this.FileSource;
                parameters.Id = new CanonicalTileId(tile.Zoom, (int)tile.TileCoordinate.x, (int)tile.TileCoordinate.y);
                parameters.MapId = _mapId;

                tile.ImageDataState = TilePropertyState.Loading;
                var rasterTile = parameters.MapId.StartsWith("mapbox://") ? new RasterTile() : new ClassicRasterTile();
                rasterTile.Initialize(parameters, (Action)(() =>
                {
                    if (rasterTile.Error != null)
                    {
                        tile.ImageDataState = TilePropertyState.Error;
                        return;
                    }
					if(!_isLayer){
							var rend = tile.GetComponent<MeshRenderer>();
							rend.material = _baseMaterial;
							tile.ImageData = new Texture2D(256, 256, TextureFormat.RGB24, false);
							tile.ImageData.wrapMode = TextureWrapMode.Clamp;
							tile.ImageData.LoadImage(rasterTile.Data);
							rend.material.mainTexture = tile.ImageData;
							tile.ImageDataState = TilePropertyState.Loaded;
					}else{
							var goPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
							goPlane.transform.SetParent(tile.transform);
							//Mesh planeMesh = goPlane.GetComponent<Mesh>();
							//goPlane.AddComponent<Mesh>();
							var mesh = new Mesh();
							var verts = new List<Vector3>();

							verts.Add((tile.Rect.min - tile.Rect.center).ToVector3xz());
							verts.Add(new Vector3(tile.Rect.xMax - tile.Rect.center.x, 0, tile.Rect.yMin - tile.Rect.center.y));
							verts.Add(new Vector3(tile.Rect.xMin - tile.Rect.center.x, 0, tile.Rect.yMax - tile.Rect.center.y));
							verts.Add((tile.Rect.max - tile.Rect.center).ToVector3xz());

							mesh.SetVertices(verts);
							var trilist = new List<int>() { 0, 1, 2, 1, 3, 2 };
							mesh.SetTriangles(trilist, 0);
							var uvlist = new List<Vector2>()
							{
								new Vector2(0,1),
								new Vector2(1,1),
								new Vector2(0,0),
								new Vector2(1,0)
							};
							mesh.SetUVs(0, uvlist);
							mesh.RecalculateNormals();
							goPlane.GetComponent<MeshFilter>().sharedMesh = mesh;
							Vector3 newPos = tile.transform.position;
							newPos.y += 20;
							goPlane.transform.position = newPos;

							var rend = goPlane.GetComponent<MeshRenderer>();
							rend.material = _baseMaterial;
							Texture2D imageData = new Texture2D(256, 256, TextureFormat.RGB24, false);
							imageData.wrapMode = TextureWrapMode.Clamp;
							imageData.LoadImage(rasterTile.Data);
							rend.material.mainTexture = imageData;
							goPlane.transform.localScale = Vector3.one;
					}
                    

                }));
            }
            else
            {
                var rend = tile.GetComponent<MeshRenderer>();
                rend.material = _baseMaterial;
            }
        }
    }
}
