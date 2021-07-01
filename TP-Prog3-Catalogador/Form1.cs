﻿using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace TP_Prog3_Catalogador
{
    public partial class Form1 : Form {

        
        private FileInfo workingDirectory;
        private NodoAdaptador nodoAdapter = new NodoAdaptador();
        private List<FileInfo> carpetasSeleccionadas = new List<FileInfo>();

        public Form1()
        {
            InitializeComponent();
            //if(workingDirectory==null)
            Nodo nodoRaiz1 = new Nodo();

            nodoAdapter.AgregarNodoRaiz("Categorias", treeView1, nodoRaiz1);
            
            treeView2.Nodes.Add("Lugares");
            label1.Text = treeView1.Nodes[0].Text;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK &&
                !String.IsNullOrWhiteSpace(folderBrowserDialog1.SelectedPath))
            {
                FormAgregarCarpetaEnCategoria formAgregarCarpetaEnCategoria = new FormAgregarCarpetaEnCategoria(nodoAdapter);

                formAgregarCarpetaEnCategoria.Directorio = new DirectoryInfo(folderBrowserDialog1.SelectedPath);               
                //formAgregarCarpetaEnCategoria.Lugares =

                formAgregarCarpetaEnCategoria.ShowDialog();
            }*/
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void agregarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String nombre = Interaction.InputBox("Indique el nombre de la nueva Categoria", "NuevaCategoria");
            if (treeView1.SelectedNode != null && !String.IsNullOrWhiteSpace(nombre)) {

                nodoAdapter.AgregarNodoHijo(nombre);
                //treeView1.SelectedNode.Nodes.Add(nombre);
            }

        }

        private void quitarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode.Text == "Categorias") return;
            nodoAdapter.QuitarNodoSeleccionado();
            nodoAdapter.CargarRaizATreeView();
            //treeView1.SelectedNode.Remove();
        }


        //devuelve lista con nombres de nodos de Categorias sin nodos hijos
        public List<String> getCategorias() {

            recursivoNodo(treeView1.Nodes[0].Nodes);
            return catDisponibles;

        }

        List<String> catDisponibles= new();
        private void recursivoNodo(TreeNodeCollection tnc)
        {
            foreach (TreeNode node in tnc)
            {
                //If we have child nodes, we'll write 
                //a parent node, then iterrate through
                //the children
                if (node.Nodes.Count > 0)
                {
                    recursivoNodo(node.Nodes);
                }
                else //No child nodes, so we just write the text
                {
                    //Si no tiene hijos podemos escribir carpetas
                    catDisponibles.Add(node.Text);
                }
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //quitar
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String jsonString;
            
            if (saveFileDialog1.ShowDialog() == DialogResult.OK && !String.IsNullOrWhiteSpace(saveFileDialog1.FileName)) 
            {
                //Escribir nodo en json:
                jsonString = JsonSerializer.Serialize<Nodo>(nodoAdapter.NodoRaiz);
                File.WriteAllText(saveFileDialog1.FileName, jsonString);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

            /*
             * Creamos una ventana de OpenFileDialog que seleccione el archivo a abrir, si apeta en OK y se tiene
             * seleccionado una rchivo, entonces se procede a leer el archivo .json y parsearlo a un objeto Nodo
             * el cual se usara como nueva Raiz del nodo principal del programa (nodoAdapter) 
             */
            if (openFileDialog1.ShowDialog() == DialogResult.OK && !String.IsNullOrWhiteSpace(openFileDialog1.FileName)) 
            {
                //Leemos el archivo y asignamos el nuevo nodo raiz, nodoadapter:
                nodoAdapter.ControlRaizCreada = false;

                String jsonString;
                jsonString = File.ReadAllText(openFileDialog1.FileName);
                Nodo nodoAuxiliar = JsonSerializer.Deserialize<Nodo>(jsonString);                
                nodoAdapter.NodoRaiz = nodoAuxiliar;

                nodoAdapter.ControlRaizCreada = true;
                //--------------------------------------------------------------

                //Cargamos el nodo al treeView 
                nodoAdapter.CargarRaizATreeView(); 

            }
                
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            
            //Se limpia la grilla
            dataGridView2.Rows.Clear();

            //al seleccionar nodo trae las carpetas asociadas a ese nodo
            List<CarpetaComentada> carpetasSeleccionadas = nodoAdapter.MapearNodoSeleccionado().CarpetasComentadas;

            if (carpetasSeleccionadas.Count() > 0)
            {
                foreach (CarpetaComentada carpeta in carpetasSeleccionadas) 
                {
                    DataGridViewRow fila = new DataGridViewRow();
                    fila.CreateCells(dataGridView2);
                    fila.Cells[0].Value = carpeta.Directorio.FullName;
                    fila.Cells[1].Value = carpeta.Comentario;
                    fila.Cells[2].Value = carpeta.numeroDeDirectoriosHijos.ToString();
                    fila.Cells[3].Value = carpeta.numeroDeArchivosHijos.ToString();
                    fila.Cells[4].Value = carpeta.TamañoEnKB;
                    dataGridView2.Rows.Add(fila);
                }

            }
            

            /*
            foreach (CarpetaComentada carpeta in carpetasSeleccionadas) 
            {
                dataGridView2.Rows.Add(carpeta);
            }*/
            

            
        }

        private void agregarCarpetaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK &&
                !String.IsNullOrWhiteSpace(folderBrowserDialog1.SelectedPath))
            {
                FormAgregarCarpetaEnCategoria formAgregarCarpetaEnCategoria = new FormAgregarCarpetaEnCategoria(nodoAdapter, nodoAdapter.MapearNodoSeleccionado());

                formAgregarCarpetaEnCategoria.Directorio = new DirectoryInfo(folderBrowserDialog1.SelectedPath);
                //formAgregarCarpetaEnCategoria.Lugares =

                formAgregarCarpetaEnCategoria.ShowDialog();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView2_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dataGridView2.CurrentRow.Cells[0].Value == null) return;
            DirectoryInfo directorioSeleccionado=new DirectoryInfo(@dataGridView2.CurrentRow.Cells[0].Value.ToString());

            treeView3.Nodes.Clear();
            LoadFolder(treeView3.Nodes, directorioSeleccionado);
            treeView3.ExpandAll();
        }
        private void LoadFolder(TreeNodeCollection nodes, DirectoryInfo folder)
        {
            var newNode = nodes.Add(folder.Name);
            foreach (var childFolder in folder.EnumerateDirectories())
            {
                LoadFolder(newNode.Nodes, childFolder);
            }
            foreach (FileInfo file in folder.EnumerateFiles())
            {
                newNode.Nodes.Add(file.Name);
            }
        }

    }
}
