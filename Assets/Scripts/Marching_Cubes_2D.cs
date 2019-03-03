using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class Marching_Cubes_2D : MonoBehaviour
{
    Mesh m;
    List<Vector3> verts;
    List<int> tris;

    static Vector2Int bounds = new Vector2Int(50, 50);

    static Vector3 mod_x = new Vector3(0.5f, 0f, 0f),
        mod_y = new Vector3(0f, 0.5f, 0f),
        mod_z = new Vector3(0f, 0, 0.5f);

    private void Start()
    {
        Render();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Render();
    }


    private void Render()
    {
        m = GetComponent<MeshFilter>().mesh;
        m.Clear();

        verts = new List<Vector3>();
        tris = new List<int>();

        List<Vertex> positions = new List<Vertex>();
        int z = 0;
        // for(int z =0;z < 8;z++)
        for (int y = 0; y < bounds.y; y++)
        {
            for (int x = 0; x < bounds.x; x++)
            {
                bool tmp = true;
                /*if (
                //TESTCASE: 3 points
                (x == 5 && y == 5)
                || (x == 4 && y == 5)
                || (x == 6 && y == 5)
                || (x == 5 && y == 4)
                || (x == 5 && y == 6)
                //TESTCASE: Four Points
                (x == 5 && y == 5)
                || (x == 4 && y == 5)
                || (x == 4 && y == 4)
                || (x == 5 && y == 4)
                || (x == 4 && y == 3)
                || (x == 3 && y == 3)
                || (x == 3 && y == 4)
                //opposite corners
                (x == 5 && y == 5)
                || (x == 4 && y == 6)
                || (x == 6 && y == 6)
                || (x == 5 && y == 7)
                )
                tmp = true;*/
                if (Random.Range(0, 2) == 0)
                {
                    tmp = false;
                }
                positions.Add(new Vertex(new Vector3(x, y, z), tmp));
            }
        }
        Trianglulate(positions);

        m.name = "New Mesh";

        //Set the vertices before the triangles.
        m.vertices = verts.ToArray();
        m.triangles = tris.ToArray();
        m.RecalculateNormals();

    }

    private void Trianglulate(List<Vertex> verts)
    {
        for(int y = 0;y < bounds.y - 1; y++)
        {
            for(int x = 0; x < bounds.x - 1; x++)
            {
                //In Unity, positive X is left.
                Vertex d_l, d_r, u_l, u_r;
                u_l = verts[x + 1 + (y + 1) * bounds.y];
                u_r = verts[x + (y + 1) * bounds.y];
                d_l = verts[x + 1 + (y + 0) * bounds.y];
                d_r = verts[x + (y + 0) * bounds.y];
                
                //To expand into 3d, we OR by 16, 32, 64, 128.
                int cellType = 0;
                if (d_l.active)
                    cellType |= 1;
                if (d_r.active)
                    cellType |= 2;
                if (u_l.active)
                    cellType |= 4;
                if (u_r.active)
                    cellType |= 8;
                //Right now we have to check for a total of 16 cases, becomes a lot more later.
                //Prob don't use a switch then.


                //In Unity, positive X is left.
                //Add triangles by upper right, bottom right, bottom left, upper left
                switch (cellType)
                {
                    //None
                    case 0:
                        break;

                    //All points
                    case 15:
                        AddQuad(u_r.pos, d_r.pos, d_l.pos, u_l.pos);
                        break;

                    //opposite corners
                    case 6:
                        AddTriangle(d_r.pos + mod_y,
                            d_r.pos,
                            d_r.pos + mod_x);
                        AddTriangle(u_l.pos - mod_x,
                            u_l.pos - mod_y,
                            u_l.pos);
                        AddQuad(u_r.pos + mod_x,
                            d_r.pos + mod_y,
                            d_r.pos + mod_x,
                            d_l.pos + mod_y
                            );
                        break;

                    case 9:
                        AddTriangle(d_l.pos - mod_x,
                            d_l.pos,
                            d_l.pos + mod_y
                            );
                        AddTriangle(u_r.pos,
                            u_r.pos - mod_y,
                            u_r.pos + mod_x);
                        AddQuad(u_r.pos + mod_x,
                            d_r.pos + mod_y,
                            d_r.pos + mod_x,
                            d_l.pos + mod_y
                            );
                        break;

                    //Corners.
                    case 1:
                        AddTriangle(d_l.pos - mod_x,
                            d_l.pos,
                            d_l.pos + mod_y
                            );
                        break;
                    case 2:
                        AddTriangle(d_r.pos + mod_y,
                            d_r.pos,
                            d_r.pos + mod_x);
                        break;
                        
                    case 4:
                        AddTriangle(u_l.pos - mod_x,
                            u_l.pos - mod_y,
                            u_l.pos);
                        break;
                    case 8:
                        AddTriangle(u_r.pos,
                            u_r.pos - mod_y,
                            u_r.pos + mod_x);
                        break;
                    
                    //2 points
                    case 3:
                        AddQuad(d_r.pos + mod_y,d_r.pos, d_l.pos, d_l.pos + mod_y);
                        break;
                    case 5:
                        AddQuad(u_l.pos - mod_x, d_l.pos - mod_x, d_l.pos, u_l.pos);
                        break;
                    case 10:
                        AddQuad(u_r.pos, d_r.pos, d_r.pos + mod_x, u_r.pos + mod_x);
                        break;
                    case 12:
                        AddQuad(u_r.pos , u_r.pos - mod_y, u_l.pos - mod_y, u_l.pos );
                        break;


                    //Three points.
                    case 7:
                        AddTriangle(u_l.pos - mod_x,
                            u_l.pos - mod_y,
                            u_l.pos);
                        AddTriangle(d_r.pos + mod_y,
                            d_l.pos + mod_y,
                            u_l.pos - mod_x);
                        AddQuad(d_r.pos + mod_y,
                            d_r.pos,
                            d_l.pos,
                            d_l.pos + mod_y);
                        break;
                    case 11:
                        AddTriangle(u_r.pos,
                            u_r.pos - mod_y,
                            u_r.pos + mod_x);
                        AddTriangle(d_r.pos + mod_y,
                            d_l.pos + mod_y,
                            u_l.pos - mod_x);
                        AddQuad(d_r.pos + mod_y,
                            d_r.pos,
                            d_l.pos,
                            d_l.pos + mod_y);
                        break;
                    case 13:
                        AddTriangle(d_l.pos - mod_x,
                            d_l.pos,
                            d_l.pos + mod_y
                            );
                        AddTriangle(d_r.pos + mod_y,
                            d_l.pos - mod_x,
                            d_l.pos + mod_y);
                        AddQuad(u_r.pos,
                            u_r.pos - mod_y,
                            u_l.pos - mod_y,
                            u_l.pos);
                        break;
                    case 14:
                        AddTriangle(d_r.pos + mod_y,
                            d_r.pos,
                            d_r.pos + mod_x);
                        AddTriangle(d_r.pos + mod_y,
                            d_l.pos - mod_x,
                            d_l.pos + mod_y);
                        AddQuad(u_r.pos,
                            u_r.pos - mod_y,
                            u_l.pos - mod_y,
                            u_l.pos);
                        break;

                }

            }
        }
    }
    

    private void AddTriangle(Vector3 x, Vector3 y, Vector3 z)
    {
        int cur_vert = verts.Count;
        verts.Add(x);
        verts.Add(y);
        verts.Add(z);
        tris.Add(cur_vert + 0);
        tris.Add(cur_vert + 1);
        tris.Add(cur_vert + 2);
    }

    //Expects upper right, lower right, lower left, upper left in that order.
    private void AddQuad(Vector3 x, Vector3 y, Vector3 z, Vector3 w)
    {
        AddTriangle(x, y, z);
        AddTriangle(x, z, w);
    }

}
