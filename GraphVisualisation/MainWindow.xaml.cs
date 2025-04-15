using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Serialization;
using System.Threading.Tasks;
using System.Data.SqlClient;

using GraphTools;


namespace GraphVisualisation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        /// <summary>
        /// Матрица смежности графа
        /// </summary>
        public List<List<TextBox>> Inputs { get;  set; }

        /// <summary>
        /// Первый выбранный граф
        /// </summary>
        public EditableGraph SelectedGraphFirst { get; set; }

        /// <summary>
        /// Второй выбранный граф
        /// </summary>
        public EditableGraph SelectedGraphSecond { get; set; }

        /// <summary>
        /// Счетчик какой граф сейчас выбирается<br></br>
        /// true == Первый<br></br>
        /// false == Второй
        /// </summary>
        public bool SelectFirst = true;

        /// <summary>
        /// Текущий граф 
        /// </summary>
        public Graph CurrentGraph { get; set; }

        /// <summary>
        /// Шанс на появление ребра, принимает значения от 0 до 100<br></br>
        /// Где 100 это 100% появление ребра, а 0 соответственно 0%
        /// </summary>
        public int Chance = 65;

        /// <summary>
        /// Список всех графов из поля графов в List
        /// </summary>
        public List<Graph> Graphs = new List<Graph>();

        public MainWindow()
        {
            InitializeComponent();
        }

        #region TextChangedEvents

        /// <summary>
        /// Обработка события изменения КОЛИЧЕСТВА ВЕРШИН ГРАФА
        /// </summary>
        void NodesCountChanged(object sender, TextChangedEventArgs e)
        {
            //Проверка что введено число больше 0 и меньше 11
            int Nodes;
            if (!int.TryParse((sender as TextBox).Text, out Nodes) || gridNodes == null)
                return;
            if (Nodes > 10 || Nodes <= 0)
                return;


           

            CreateInputNodes(Nodes);

            Graph NewCurrentGraph = new Graph(CreateListFromInput(), 150);
            SetNewCurrentGraph(NewCurrentGraph);
            
        }

        /// <summary>
        /// Обработка события изменения РЕБЕР ГРАФА
        /// </summary>
        void InputEdgesChanged(object sender, TextChangedEventArgs e)
                {
                    Graph NewCurrentGraph = new Graph(CreateListFromInput(), 150);
                    SetNewCurrentGraph(NewCurrentGraph);
                }

        #endregion TextChangedEvents

        #region Buttons

        /// <summary>
        /// Обработка кнопки "ЗАПОЛНИТЬ СЛУЧАЙНО"
        /// По умолчанию шанс на появления ребра равен 65% в поле Chance
        /// </summary>
       async void RandomFillButton(object sender, RoutedEventArgs e)
        {
            if (CurrentGraph == null)
            {
                MessageBox.Show("Текущий граф пуст..");
                return;
            }
            await Task.Run(() =>
            {
                Random random = new Random();
                int winner;

                for (int i = 0; i < Inputs.Count; i++)
                {
                    for (int j = 0; j < Inputs.Count; j++)
                    {
                        if (i == j)
                            continue;
                        winner = random.Next(0, 100);
                        var text = winner >= Chance ? "1" : "0";
                        Inputs[i][j].Dispatcher.Invoke(() => Inputs[i][j].Text = text);
                    }
                }
            });
        }

        /// <summary>
        /// Обработка кнопки "ДОБАВИТЬ ГРАФ"
        /// </summary>
        void AddGraphButton(object sender , RoutedEventArgs e )
            {
                List<List<int>> Edges = CreateListFromInput();
                if (Edges == null)
                {
                    MessageBox.Show("Пустое количество вершин");
                    return;
                }
                EditableGraph graph = new EditableGraph(Edges);
                AddGraph(graph);

            }

        /// <summary>
        /// Обработка кнопки "МЕНЮ ДЕЙСТВИЙ"
        /// </summary>
        void ActionsMenuOpenButton(object sender, RoutedEventArgs e)
        {
            ActionsMenuDialog actionsMenuDialog = new ActionsMenuDialog();
            actionsMenuDialog.ShowDialog();
            switch (actionsMenuDialog.Action)
            {
                case 1:
                {
                    AdditionGraph();
                    return;
                }
                case 2:
                {
                    SubstractionGraph();
                    return;
                }
                case 3:
                {
                    ImportGraph();
                    return;
                }
                case 4:
                {
                    SaveGraph();
                    return;
                }
                case 5:
                {
                    DeepFirstSearchGraph();
                    return;
                }
                case 6:
                {
                    return;
                }

            }

        }

        #endregion Buttons

        #region GraphEvents

        /// <summary>
        /// Обработка события "УДАЛЕНИЕ" графа
        /// </summary>
        void RemoveGraphEvent(object sender, EventArgs e)
        {
            RemoveGraph((EditableGraph)sender);
        }

        /// <summary>
        /// Обработка события "ВЫБОР" графа
        /// </summary>
        void SelectGraphEvent(object sender, EventArgs e)
        {
            EditableGraph graph = (EditableGraph)sender;
            if (SelectFirst)
            {
                if (SelectedGraphFirst != null)
                    GraphUtils.HighlightGraph(SelectedGraphFirst, Brushes.Black);
                SelectedGraphFirst = graph;
                GraphUtils.HighlightGraph(SelectedGraphFirst, Brushes.Blue);
                SelectFirst = false;
            }
            else
            {
                if (SelectedGraphSecond != null && SelectedGraphFirst != SelectedGraphSecond)
                    GraphUtils.HighlightGraph(SelectedGraphSecond, Brushes.Black);
                SelectedGraphSecond = graph;
                GraphUtils.HighlightGraph(SelectedGraphSecond, Brushes.Red);
                SelectFirst = true;
            }
            
        }
        /// <summary>
        /// Обработка события "СДЕЛАТЬ ТЕКУЩИМ" графа
        /// </summary>
        void CurrentGraphEvent(object sender, EventArgs e)
        {
            
            Graph NewCurrentGraph = (Graph)sender;

            //Если текущим становится один из выбранных, то выделение снимается с обоих
            if (NewCurrentGraph == SelectedGraphFirst || NewCurrentGraph == SelectedGraphSecond)
            {
                GraphUtils.ClearSelection(gridGraphs.Children);
                SelectedGraphFirst = null;
                SelectedGraphSecond = null;
                SelectFirst = true;
            }


            SetNewCurrentGraph(NewCurrentGraph);

            CreateInputNodes(CurrentGraph.Nodes);

            CreateInputFromList(CurrentGraph.Edges);
            
        }

        #endregion GraphEvents

        #region ActionsMenu

        /// <summary>
        /// Функция запускающая сложение графов
        /// </summary>
        void AdditionGraph()
        {
            if (SelectedGraphFirst == null)
            {
                MessageBox.Show("Вы не выбрали первый граф");
                return;
            }
            if (SelectedGraphSecond == null)
            {
                MessageBox.Show("Вы не выбрали второй граф");
                return;
            }
            EditableGraph selectedFirst = new EditableGraph(SelectedGraphFirst.Edges);
            EditableGraph selectedSecond = new EditableGraph(SelectedGraphSecond.Edges);
            EditableGraph result = new EditableGraph(GraphUtils.Addiction(selectedFirst, selectedSecond).Edges);
            AddGraph(result);
        }

        /// <summary>
        /// Функция запускающая вычитание графов
        /// </summary>
        void SubstractionGraph()
        {
            if (SelectedGraphFirst == null)
            {
                MessageBox.Show("Вы не выбрали первый граф");
                return;
            }
            if (SelectedGraphSecond == null)
            {
                MessageBox.Show("Вы не выбрали второй граф");
                return;
            }
            EditableGraph selectedFirst = new EditableGraph(SelectedGraphFirst.Edges);
            EditableGraph selectedSecond = new EditableGraph(SelectedGraphSecond.Edges);
            EditableGraph result = new EditableGraph(GraphUtils.Substraction(selectedFirst, selectedSecond).Edges);
            AddGraph(result);
        }

        /// <summary>
        /// Функция запускаяющая поиск в глубину
        /// </summary>
        void DeepFirstSearchGraph()
        {
            if (CurrentGraph == null)
            {
                MessageBox.Show("Текущий граф пуст..");
                return;
            }
            DfsDialog dialog = new DfsDialog();
            if (dialog.ShowDialog()==true)
            {
                Graph NewCurrentGraph =  GraphUtils.DeepFirstSearch(CurrentGraph, int.Parse(dialog.StartNode.Text));
                if (!(CurrentGraph is null))
                    canvas.Children.Remove(CurrentGraph);
                CurrentGraph = NewCurrentGraph;
                canvas.Children.Add(CurrentGraph);
                Canvas.SetLeft(CurrentGraph, 1255);
                Canvas.SetTop(CurrentGraph, 95);
                CreateInputFromList(CurrentGraph.Edges);
            }
            
           
        }

        /// <summary>
        /// Функция реализующее логику сохранения ТЕКУЩЕГО графа
        /// </summary>
        void SaveGraph()
        {
            if (CurrentGraph == null)
            {
                MessageBox.Show("Текущий граф пуст, нечего сохранять");
                return;
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog()==true)
            {
                string filePath = saveFileDialog.FileName;
                string xml = SerializeToXml(CurrentGraph);
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.Write(xml);
                }
            }   

        }

        /// <summary>
        /// Функция реализовывающая логику импорта графа
        /// </summary>
        void ImportGraph()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                using (StreamReader reader = new StreamReader(filePath))
                {
                    
                    AddGraph(new EditableGraph(DeserializeFromXml(reader.ReadToEnd())));
                }
            }
        }

        #endregion ActionsMenu

        #region Utils

        /// <summary>
        /// Добавляет граф в список графов
        /// </summary>
        /// <param name="graph">Добавляемый граф</param>
        void AddGraph(EditableGraph graph)
        {

            int row = gridGraphs.Children.Count / 4;
            int column = gridGraphs.Children.Count % 4;

            gridGraphs.Children.Add(graph);
            Graphs.Add(graph);

            if (row > gridGraphs.RowDefinitions.Count - 1)
            {
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(205);
                gridGraphs.RowDefinitions.Add(rowDefinition);
            } 
            graph.RemoveGraph += RemoveGraphEvent;
            graph.SelectGraph += SelectGraphEvent;
            graph.CurrentGraph += CurrentGraphEvent;
            Grid.SetColumn(graph, column);
            Grid.SetRow(graph, row);

        }

        /// <summary>
        /// Удаляет граф из списка графов
        /// </summary>
        /// <param name="graph"></param>
        void RemoveGraph(EditableGraph graph)
        {
            if (graph == CurrentGraph)
            {
                CreateInputNodes(graph.Edges.Count);
                Inputs = new List<List<TextBox>>();
            }
            if (graph == SelectedGraphFirst || graph == SelectedGraphSecond)
            {
                GraphUtils.ClearSelection(gridGraphs.Children);
                SelectedGraphFirst = null;
                SelectedGraphSecond = null;
                SelectFirst = true;
            }
            if (graph == SelectedGraphFirst)
                SelectedGraphFirst = null;
            else if (graph == SelectedGraphSecond)
                SelectedGraphSecond = null;

            gridGraphs.Children.Remove(graph);
            Graphs.Remove(graph);

            for (int i = 0; i < gridGraphs.Children.Count; i++)
            {
                UIElement move = gridGraphs.Children[i];
                Grid.SetRow(move, i / 4);
                Grid.SetColumn(move, i % 4);
            }
        }

        /// <summary>
        /// Создание TextBox для ввода ребер графа <br></br>
        /// Каждый из которых означает есть ли ребро i->j, где i и j от 0 до Nodes
        /// </summary>
        /// <param name="Nodes">Количество вершин графа</param>
        void CreateInputNodes(int Nodes)
        {
            Inputs = new List<List<TextBox>>();
            gridNodes.Children.Clear();
            gridNodes.RowDefinitions.Clear();
            for (int i = 0; i < Nodes; i++)
            {
                Inputs.Add(new List<TextBox>());
                for (int j = 0; j < Nodes; j++)
                {
                    RowDefinition rowDefinition = new RowDefinition();
                    rowDefinition.Height = new GridLength(35);
                    gridNodes.RowDefinitions.Add(rowDefinition);


                    TextBox textBox = new TextBox
                    {
                        Height = 30,
                        Width = 100,
                        FontSize = 15,
                        Text = "0"
                    };
                    textBox.TextChanged += new TextChangedEventHandler(this.InputEdgesChanged);
                    Label label = new Label
                    {
                        Content = String.Format("{0}->{1}", i, j),
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        Height = 30,
                        Width = 100,
                        FontSize = 15
                    };

                    Grid.SetRow(textBox, i*Nodes+j);
                    Grid.SetColumn(textBox, 1);

                    Grid.SetRow(label, i*Nodes+j);
                    Grid.SetColumn(label, 0);

                    gridNodes.Children.Add(textBox);
                    gridNodes.Children.Add(label);

                    Inputs[i].Add(textBox);
                }
            }
        }

        /// <summary>
        /// Создание списка смежности из двумерного массива Inputs
        /// </summary>
        /// <returns></returns>
        List<List<int>> CreateListFromInput()
        {
            List<List<int>> newList = new List<List<int>>();
            if (Inputs == null)
                return null;
            for (int i = 0; i < Inputs.Count(); i++)
            {
                List<int> rowValues = new List<int>();
                for (int j = 0; j < Inputs[i].Count(); j++)
                {
                    if (Inputs[i][j].Text == "0")
                        continue;
                    if (Inputs[i][j].Text != "0")
                    {
                        rowValues.Add(j);
                    }
                }
                newList.Add(rowValues);
            }
            return newList;
        }

        /// <summary>
        /// Заполнение двумерного массива Inputs из переданного списка смежности
        /// </summary>
        void CreateInputFromList(List<List<int>> lists)
        {
            
            for (int i = 0;i!=Inputs.Count;i++)
            {
                for (int j = 0; j != Inputs[i].Count();j++)
                {
                    Inputs[i][j].TextChanged -= new TextChangedEventHandler(InputEdgesChanged);
                    Inputs[i][j].Text = "0";
                    Inputs[i][j].TextChanged += new TextChangedEventHandler(InputEdgesChanged);
                }
            }
            for (int i = 0; i != lists.Count; i++)
            {
                for (int j = 0; j != lists[i].Count; j++)
                {
                    Inputs[i][lists[i][j]].TextChanged -= new TextChangedEventHandler(InputEdgesChanged);
                    Inputs[i][lists[i][j]].Text = "1";
                    Inputs[i][lists[i][j]].TextChanged += new TextChangedEventHandler(InputEdgesChanged);
                }
            }
        }

        /// <summary>
        /// Создание и добавление нового текущего графа
        /// </summary>
        /// <param name="graph"></param>
        void SetNewCurrentGraph(Graph graph)
        {
            if (!(CurrentGraph is null))
                canvas.Children.Remove(CurrentGraph);
            CurrentGraph = new Graph(graph, 150);
            canvas.Children.Add(CurrentGraph);
            Canvas.SetLeft(CurrentGraph, 1255);
            Canvas.SetTop(CurrentGraph, 95);
            
        }
        /// <summary>
        /// Сериализует граф по его списку смежности в XML строку
        /// </summary>
        /// <param name="graph"></param>
        /// <returns>XML представление списка смежности</returns>
        string SerializeToXml(Graph graph)
        {
            XmlSerializer Serializer = new XmlSerializer(typeof(List<List<int>>));
            using (StringWriter stringWriter = new StringWriter())
            {
                Serializer.Serialize(stringWriter, graph.Edges);
                return stringWriter.ToString();
            }

        }
        /// <summary>
        /// Создает граф из предоставленного XML 
        /// </summary>
        /// <param name="xml"></param>
        /// <returns>Граф из предоставленного XML</returns>
        Graph DeserializeFromXml(string xml)
        {
            XmlSerializer Serializer = new XmlSerializer(typeof(List<List<int>>));
            using (StringReader stringReader = new StringReader(xml))
            {
                return new Graph((List<List<int>>)Serializer.Deserialize(stringReader));
            }
        }
        #endregion Utils

        #region DataBase

        
        /// <summary>
        /// Соединение с базой данных в формате String
        /// </summary>
        public string Connection = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=" + Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString())+"\\DataBase\\GraphDataBase.mdf;Integrated Security = True";
        /// <summary>
        /// Загрузить базу данных
        /// </summary>
        void LoadDataBase()
        {

            using (SqlConnection connection = new SqlConnection(Connection))
            {
                connection.Open();

                string sqlQuery = "SELECT ID, Radius FROM Graphs";

                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            int radius = reader.GetInt32(1);

                            if (radius == 150)
                            {
                                Graph newCurrentGraph = LoadGraphFromDataBase(id);      
                                inputNodes.Text = newCurrentGraph.Nodes.ToString();
                                SetNewCurrentGraph(newCurrentGraph);
                                CreateInputNodes(newCurrentGraph.Nodes);
                                CreateInputFromList(newCurrentGraph.Edges);
                           

                            }

                            else
                                AddGraph(new EditableGraph(LoadGraphFromDataBase(id)));
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Сохранить базу данных
        /// </summary>
        void SaveDataBase()
        {
            foreach (Graph graph in Graphs)
            {
                SaveGraphToDataBase(graph);
            }
            SaveGraphToDataBase(CurrentGraph);
        }
        /// <summary>
        /// Удалить всё из базы данных
        /// </summary>
        void RemoveAllFromDataBase()
        {
            string sqlQuery = "DELETE FROM Graphs";

           
            using (SqlConnection connection = new SqlConnection(Connection))
            {
                
                connection.Open();

                
                using (SqlCommand DeleteCommand = new SqlCommand(sqlQuery, connection))
                {
                   
                    DeleteCommand.ExecuteNonQuery();
                }

                string ResetQuery = "DBCC CHECKIDENT ('Graphs', RESEED, 0)";
                using (SqlCommand ResetCommand = new SqlCommand(ResetQuery, connection))
                {
                    ResetCommand.ExecuteNonQuery();
                }

                connection.Close();
            }
        }
        /// <summary>
        /// Сохранить граф в базу данных
        /// </summary>
        /// <param name="graph"></param>
        public void SaveGraphToDataBase(Graph graph)
        {
            string xml = SerializeToXml(graph);
            using (SqlConnection connection = new SqlConnection(Connection))
            {
                string query = "INSERT INTO Graphs (Graph, Radius) VALUES (@Graph,@Radius)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Graph", xml);
                    command.Parameters.AddWithValue("@Radius", graph.Radius);
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }
        /// <summary>
        /// Загрузить граф по ID из базы данных
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Graph LoadGraphFromDataBase(int id)
        {
            using (SqlConnection connection = new SqlConnection(Connection))
            {
                string query = "SELECT Graph FROM Graphs WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    connection.Open();
                    string xml = (string)command.ExecuteScalar();
                    connection.Close();

                    return DeserializeFromXml(xml);
                }
            }
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            RemoveAllFromDataBase();
            SaveDataBase();
        }
        private void Window_Initialized(object sender, EventArgs e)
        {
            LoadDataBase();
        }

        #endregion DataBase

      
    }
}
