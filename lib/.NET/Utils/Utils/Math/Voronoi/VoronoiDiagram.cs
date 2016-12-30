using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using Delaunay;
using Delaunay.LR;

using StarfieldUtils.MathUtils;

/** Unity-friendly Output from runnning the ported AS3Delaunay library

"VoronoiNS" is a silly namespace name, but C# is buggy and can't cope with a class and namespace with same
name, and the original port (ab)uses Voronoi as a class-name
  */
namespace VoronoiNS
{
    public class VoronoiDiagram
    {
        List<VoronoiCell> cells = new List<VoronoiCell>();
        List<VoronoiCellEdge> edges = new List<VoronoiCellEdge>();
        List<VoronoiCellVertex> vertices = new List<VoronoiCellVertex>();

        /**
        Stupidly, Unity in 2D mode uses co-ords incompatible with Unity in 3D mode (xy instead of xz).

        So we have to provide a boolean to switch between the two modes!
        */
        public static VoronoiDiagram CreateDiagramFromVoronoiOutput(Voronoi voronoiGenerator, bool useUnity2DCoordsNot3D)
        {
            VoronoiDiagram map = go.AddComponent<VoronoiDiagram>();

            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Reset();
            watch.Start();

            Dictionary<Site, VoronoiCell> generatedCells = new Dictionary<Site, VoronoiCell>();
            Dictionary<Vec2D, VoronoiCellVertex> generatedVertices = new Dictionary<Vec2D, VoronoiCellVertex>();

            int numEdgesCreated = 0;
            foreach (Edge edge in voronoiGenerator.Edges())
            {
                GameObject goEdge = new GameObject("Edge-#" + (numEdgesCreated++));
                goEdge.transform.parent = goEdgeHolder.transform;

                VoronoiCellEdge vEdge = goEdge.AddComponent<VoronoiCellEdge>();
                Debugger.Log(0, "[INFO]", "Processing edge = " + edge + " with clippedEnds = " + (edge.clippedEnds == null ? "null" : "" + edge.clippedEnds.Count));
                if (!edge.visible)
                {
                    Debugger.Log(0, "[INFO]", "...Voronoi algorithm generated a non-existent edge, skipping it...");
                    continue;
                }
                Vec2D clippedEndLeft = (Vec2D)edge.clippedEnds[Side.LEFT];
                Vec2D clippedEndRight = (Vec2D)edge.clippedEnds[Side.RIGHT];
                vEdge.AddVertex(FetchVertexOrAddToObject(clippedEndLeft, generatedVertices, goVertexHolder, useUnity2DCoordsNot3D));
                vEdge.AddVertex(FetchVertexOrAddToObject(clippedEndRight, generatedVertices, goVertexHolder, useUnity2DCoordsNot3D));
                goEdge.transform.localPosition = (vEdge.edgeVertexA.transform.localPosition + vEdge.edgeVertexB.transform.localPosition) / 2.0f;

                Site[] bothSites = new Site[] { edge.leftSite, edge.rightSite };
                foreach (Site site in bothSites)
                {
                    /** Re-use or create the Cell */
                    VoronoiCell newCell = null; // C# is rubbish. Crashes if Dictionary lacks the key. Very bad design.
                    if (generatedCells.ContainsKey(site))
                        newCell = generatedCells[site];
                    GameObject goCell;

                    if (newCell == null)
                    {
                        goCell = new GameObject("Cell-#" + generatedCells.Count);
                        goCell.transform.parent = goCellHolder.transform;
                        goCell.transform.localPosition = new Vec3D(site.Coord.X, useUnity2DCoordsNot3D ? site.Coord.Y : 0, useUnity2DCoordsNot3D ? 0 : site.Coord.Y);
                        newCell = goCell.AddComponent<VoronoiCell>();
                        generatedCells.Add(site, newCell);
                    }
                    else
                    {
                        goCell = newCell.gameObject;
                    }

                    /** Now that cell is created, attach it to BOTH Vertex's on the new Edge
                              (so that later its easy for us to to find Cells-for-this-Vertex, and also:
                              for a Cell to "trace" around its edges, picking "next edge" from each Vertex
                              by looking at which edges from the vertex border this cell (by checking
                              that BOTH Vertex-ends of the edge are attached to this cell)
                    
                              Also add the edge itself to the cell, and the cell to the edge.
                              */
                    vEdge.AddCellWithSideEffects(newCell);

                }
            }
            watch.Stop();

            return map;
        }

        /** Required because of major bugs in the AS3Delaunay algorithm where it fails
        to maintain identity of Vector2 co-ordinates (unlike Unity, which maintains them
        effectively).

        Any data that came from the AS3Delaunay core ported code will return "randomly varying"
        float co-ords, that we have to manually error-correct before using as a key in dictionary,
        Ugh.
        */
        private static VoronoiCellVertex FetchVertexOrAddToObject(Vec2D voronoiSite, Dictionary<Vec2D, VoronoiCellVertex> verticesByVector2, GameObject go, bool useUnity2DCoordsNot3D)
        {
            foreach (KeyValuePair<Vec2D, VoronoiCellVertex> item in verticesByVector2)
            {
                if (Site.CloseEnough(item.Key, voronoiSite))
                    return item.Value;
            }

            /** no match found; create, add, return */
            GameObject goVertex = new GameObject("Vertex-#" + (verticesByVector2.Count));
            VoronoiCellVertex newVertex = goVertex.AddComponent<VoronoiCellVertex>();
            newVertex.positionInDiagram = voronoiSite;
            verticesByVector2.Add(voronoiSite, newVertex);
            goVertex.transform.parent = go.transform;
            goVertex.transform.localPosition = new Vec3D(voronoiSite.X, useUnity2DCoordsNot3D ? voronoiSite.Y : 0, useUnity2DCoordsNot3D ? 0 : voronoiSite.Y); // easier to find it when debugging!

            return newVertex;
        }

        public VoronoiCell[] Cells
        {
            get
            {
                return cells.ToArray();
            }
        }

        public VoronoiCellEdge[] Edges
        {
            get
            {
                return edges.ToArray();
            }
        }
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}