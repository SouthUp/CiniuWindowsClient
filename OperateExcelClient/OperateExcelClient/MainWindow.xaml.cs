using OperateExcelClient.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OperateExcelClient
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        string strFileName = "";
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            element.Source = AppDomain.CurrentDomain.BaseDirectory + @"Resources\Gif\loading.gif";
        }
        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Excel文件(*.xlsx)|*.xlsx";
            ofd.ValidateNames = true;
            ofd.CheckPathExists = true;
            ofd.CheckFileExists = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                strFileName = ofd.FileName;
                FilePathTextBox.Text = strFileName;
            }
        }
        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(strFileName))
            {
                System.Windows.MessageBox.Show("请选择要导入的Excel模板");
            }
            else
            {
                BusyGrid.Visibility = Visibility.Visible;
                Task.Run(() =>
                {
                    APIService service = new APIService();
                    string userName = ConfigurationManager.AppSettings["UserName"].ToString();
                    string passWord = ConfigurationManager.AppSettings["PassWord"].ToString();
                    string token = service.LoginIn(userName, passWord);
                    if (!string.IsNullOrEmpty(token))
                    {
                        ExcuteImpData(token);
                    }
                    else
                    {
                        Dispatcher.Invoke(() => {
                            BusyGrid.Visibility = Visibility.Collapsed;
                        });
                        System.Windows.MessageBox.Show("配置用户登陆无效");
                    }
                });
            }
        }
        private async void ExcuteImpData(string token)
        {
            int dbCatograyCount = 0;
            int dbLawClauseCount = 0;
            int dbWordsCount = 0;
            int dbWordsRelationCount = 0;

            int dbCatograyTotalCount = 0;
            int dbLawClauseTotalCount = 0;
            int dbWordsTotalCount = 0;
            int dbWordsRelationTotalCount = 0;
            Task task = Task.Run(() =>
            {
                try
                {
                    Aspose.Cells.Workbook workbookName = new Aspose.Cells.Workbook(strFileName);
                    int sheetCount = workbookName.Worksheets.Count;
                    for (int iSheet = 0; iSheet < sheetCount; iSheet++)
                    {
                        string sheetName = workbookName.Worksheets[iSheet].Name;
                        if (sheetName == "Type表")
                        {
                            Aspose.Cells.Cells cellsName1 = workbookName.Worksheets[iSheet].Cells;
                            int minDataRow = cellsName1.MinDataRow;
                            int minDataColumn = cellsName1.MinDataColumn;
                            int maxDataRow = cellsName1.MaxDataRow;
                            int maxDataColumn = cellsName1.MaxDataColumn;

                            int typeId = 0;
                            int typeName = 0;
                            for (int j = minDataColumn; j < maxDataColumn + 1; j++)
                            {
                                string s = cellsName1[minDataRow, j].StringValue.Trim();
                                if (s == "Type ID")
                                {
                                    typeId = j;
                                }
                                else if (s == "Type Name")
                                {
                                    typeName = j;
                                }
                            }
                            //导入数据
                            for (int i = minDataRow + 1; i < maxDataRow + 1; i++)
                            {
                                try
                                {
                                    WordsCategoryInfo info = new WordsCategoryInfo();
                                    info.typeId = cellsName1[i, typeId].StringValue.Trim();
                                    info.typeName = cellsName1[i, typeName].StringValue.Trim();
                                    APIService service = new APIService();
                                    service.ImpWordsCategoryData(info, token);
                                    dbCatograyTotalCount++;
                                }
                                catch (Exception ex)
                                {
                                    dbCatograyCount++;
                                }
                                Dispatcher.Invoke(() => {
                                    DBCatograyCount.Text = dbCatograyCount.ToString() + "( 累计" + dbCatograyTotalCount + "条数据)";
                                });
                            }
                        }
                        else if (sheetName == "LawClause")
                        {
                            Aspose.Cells.Cells cellsName1 = workbookName.Worksheets[iSheet].Cells;
                            int minDataRow = cellsName1.MinDataRow;
                            int minDataColumn = cellsName1.MinDataColumn;
                            int maxDataRow = cellsName1.MaxDataRow;
                            int maxDataColumn = cellsName1.MaxDataColumn;

                            int clauseID = 0;
                            int decription = 0;
                            int typeId = 0;
                            for (int j = minDataColumn; j < maxDataColumn + 1; j++)
                            {
                                string s = cellsName1[minDataRow, j].StringValue.Trim();
                                if (s == "Clause ID")
                                {
                                    clauseID = j;
                                }
                                else if (s == "decription")
                                {
                                    decription = j;
                                }
                                else if (s == "Type ID")
                                {
                                    typeId = j;
                                }
                            }
                            //导入数据
                            for (int i = minDataRow + 1; i < maxDataRow + 1; i++)
                            {
                                try
                                {
                                    LawClauseInfo info = new LawClauseInfo();
                                    info.clauseId = cellsName1[i, clauseID].StringValue.Trim();
                                    info.description = cellsName1[i, decription].StringValue.Trim();
                                    info.typeId = cellsName1[i, typeId].StringValue.Trim();
                                    APIService service = new APIService();
                                    service.ImpLawClauseData(info, token);
                                    dbLawClauseTotalCount++;
                                }
                                catch (Exception ex)
                                {
                                    dbLawClauseCount++;
                                }
                                Dispatcher.Invoke(() => {
                                    DBLawClauseCount.Text = dbLawClauseCount.ToString() + "( 累计" + dbLawClauseTotalCount + "条数据)";
                                });
                            }
                        }
                        else if (sheetName == "Word")
                        {
                            Aspose.Cells.Cells cellsName1 = workbookName.Worksheets[iSheet].Cells;
                            int minDataRow = cellsName1.MinDataRow;
                            int minDataColumn = cellsName1.MinDataColumn;
                            int maxDataRow = cellsName1.MaxDataRow;
                            int maxDataColumn = cellsName1.MaxDataColumn;

                            int wordID = 0;
                            int name = 0;
                            int sensitive = 0;
                            int clauseID = 0;
                            for (int j = minDataColumn; j < maxDataColumn + 1; j++)
                            {
                                string s = cellsName1[minDataRow, j].StringValue.Trim();
                                if (s == "Word ID")
                                {
                                    wordID = j;
                                }
                                else if (s == "name")
                                {
                                    name = j;
                                }
                                else if (s == "sensitive")
                                {
                                    sensitive = j;
                                    clauseID = sensitive + 1;
                                }
                            }
                            //导入词数据
                            for (int i = minDataRow + 1; i < maxDataRow + 1; i++)
                            {
                                try
                                {
                                    WordsInfo info = new WordsInfo();
                                    info.wordId = cellsName1[i, wordID].StringValue.Trim();
                                    info.name = cellsName1[i, name].StringValue.Trim();
                                    info.sensitive = cellsName1[i, sensitive].StringValue.Trim() == "Y" ? true : false;
                                    info.official = true;
                                    info.comment = "";
                                    APIService service = new APIService();
                                    service.ImpWordsData(info, token);
                                    dbWordsTotalCount++;
                                }
                                catch (Exception ex)
                                {
                                    dbWordsCount++;
                                }
                                Dispatcher.Invoke(() =>
                                {
                                    DBWordsCount.Text = dbWordsCount.ToString() + "( 累计" + dbWordsTotalCount + "条数据)";
                                });
                            }
                            //导入关系数据
                            for (int i = minDataRow + 1; i < maxDataRow + 1; i++)
                            {
                                try
                                {
                                    WordsRelationInfo info = new WordsRelationInfo();
                                    info.clauseIds = new List<string>();
                                    info.wordId = cellsName1[i, wordID].StringValue.Trim();
                                    for (int j = clauseID; j < maxDataColumn + 1; j++)
                                    {
                                        string clauseId = cellsName1[i, j].StringValue.Trim();
                                        if (!string.IsNullOrEmpty(clauseId))
                                        {
                                            if (!info.clauseIds.Contains(clauseId))
                                                info.clauseIds.Add(clauseId);
                                        }
                                    }
                                    APIService service = new APIService();
                                    service.ImpWordsRelationData(info, token);
                                    dbWordsRelationTotalCount++;
                                }
                                catch (Exception ex)
                                {
                                    dbWordsRelationCount++;
                                }
                                Dispatcher.Invoke(() =>
                                {
                                    DBWordsRelationCount.Text = dbWordsRelationCount.ToString() + "( 累计" + dbWordsRelationTotalCount + "条数据)";
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                { }
                Dispatcher.Invoke(() => {
                    BusyGrid.Visibility = Visibility.Collapsed;
                });
            });
            await task;
        }
    }
}
