using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Malom
{
    public partial class Gui : Form
    {
        private Logic _Logika = new Logic();
        private GuiField[] _GuiFields;
        private const Int32 _FIELDSIZE = 15;

        public Gui() // magamnak: istenem milyen pepecselős borzalom volt ez
        {
            InitializeComponent();
            _GuiFields = new GuiField[24];
            _GuiFields[0] = new GuiField(_Logika,7,0,new Rectangle(0,2,_FIELDSIZE,_FIELDSIZE));
            _GuiFields[1] = new GuiField(_Logika, 0, 0, new Rectangle(boardpanel.Width/2-9, 0, _FIELDSIZE, _FIELDSIZE));
            _GuiFields[2] = new GuiField(_Logika, 1, 0, new Rectangle(boardpanel.Width-19, 2, _FIELDSIZE, _FIELDSIZE));
            _GuiFields[3] = new GuiField(_Logika, 7, 1, new Rectangle((boardpanel.Width/7)*1+5, (boardpanel.Height/7)*1+8, _FIELDSIZE, _FIELDSIZE));
            _GuiFields[4] = new GuiField(_Logika, 0, 1, new Rectangle(boardpanel.Width / 2 - 9, (boardpanel.Height / 7) * 1 + 5, _FIELDSIZE, _FIELDSIZE));
            _GuiFields[5] = new GuiField(_Logika, 1, 1, new Rectangle((boardpanel.Width / 7) * 6 - 24, (boardpanel.Height / 7) * 1 + 7, _FIELDSIZE, _FIELDSIZE));
            _GuiFields[6] = new GuiField(_Logika, 7, 2, new Rectangle((boardpanel.Width / 7) * 2 + 4, (boardpanel.Height/7)*2+7, _FIELDSIZE, _FIELDSIZE));
            _GuiFields[7] = new GuiField(_Logika, 0, 2, new Rectangle(boardpanel.Width / 2 - 9, (boardpanel.Height/7)*2+7, _FIELDSIZE, _FIELDSIZE));
            _GuiFields[8] = new GuiField(_Logika, 1, 2, new Rectangle((boardpanel.Width / 7) * 4 + 15, (boardpanel.Height/7)*2+7, _FIELDSIZE, _FIELDSIZE));
            _GuiFields[9] = new GuiField(_Logika, 6, 0, new Rectangle(0, boardpanel.Height/2-9, _FIELDSIZE, _FIELDSIZE));
            _GuiFields[10] = new GuiField(_Logika, 6, 1, new Rectangle((boardpanel.Width / 7) * 1 + 4, boardpanel.Height/2-10, _FIELDSIZE, _FIELDSIZE));
            _GuiFields[11] = new GuiField(_Logika, 6, 2, new Rectangle((boardpanel.Width/7)*2 +6, boardpanel.Height/2-10, _FIELDSIZE, _FIELDSIZE));
            _GuiFields[12] = new GuiField(_Logika, 2, 2, new Rectangle((boardpanel.Width/7)*4+15, boardpanel.Height/2-10, _FIELDSIZE, _FIELDSIZE));
            _GuiFields[13] = new GuiField(_Logika, 2, 1, new Rectangle((boardpanel.Width/7)*6-21, boardpanel.Height/2-9, _FIELDSIZE, _FIELDSIZE));
            _GuiFields[14] = new GuiField(_Logika, 2, 0, new Rectangle(boardpanel.Width - 17, boardpanel.Height/2-10, _FIELDSIZE, _FIELDSIZE));
            _GuiFields[15] = new GuiField(_Logika, 5, 2, new Rectangle((boardpanel.Width / 7) * 2 + 4, (boardpanel.Height/7)*4+15, _FIELDSIZE, _FIELDSIZE));
            _GuiFields[16] = new GuiField(_Logika, 4, 2, new Rectangle(boardpanel.Width/2-9, (boardpanel.Height/7)*4+16, _FIELDSIZE, _FIELDSIZE));
            _GuiFields[17] = new GuiField(_Logika, 3, 2, new Rectangle((boardpanel.Width/7)*4+16, (boardpanel.Height/7)*4+16, _FIELDSIZE, _FIELDSIZE));
            _GuiFields[18] = new GuiField(_Logika, 5, 1, new Rectangle((boardpanel.Width/7)*1+5, (boardpanel.Height/7)*6-27, _FIELDSIZE, _FIELDSIZE));
            _GuiFields[19] = new GuiField(_Logika, 4, 1, new Rectangle(boardpanel.Width/2-8, (boardpanel.Height/7)*6-27, _FIELDSIZE, _FIELDSIZE));
            _GuiFields[20] = new GuiField(_Logika, 3, 1, new Rectangle((boardpanel.Width/7)*6-23, (boardpanel.Height/7)*6-27, _FIELDSIZE, _FIELDSIZE));
            _GuiFields[21] = new GuiField(_Logika, 5, 0, new Rectangle(5, boardpanel.Height-21, _FIELDSIZE, _FIELDSIZE));
            _GuiFields[22] = new GuiField(_Logika, 4, 0, new Rectangle(boardpanel.Width / 2 - 9, boardpanel.Height-18, _FIELDSIZE, _FIELDSIZE));
            _GuiFields[23] = new GuiField(_Logika, 3, 0, new Rectangle(boardpanel.Width - 17, boardpanel.Height-21, _FIELDSIZE, _FIELDSIZE));
        }

        private bool _Removing = false;
        private GuiField FromField = null;

        /// <summary>
        /// Panel eseményvezérlő
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void boardpanel_MouseDown(object sender, MouseEventArgs e)
        {
            Rectangle clickPos = new Rectangle(e.X, e.Y, 0, 0);
            Graphics gr = boardpanel.CreateGraphics();
            foreach (GuiField i in _GuiFields)
            {
                if (i.GuiPos.Contains(clickPos))
                {
                    if (_Removing == true)
                    {
                        if(!_Logika.RemovePiece(i.LogicX,i.LogicY, _Logika.ActivePlayer))
                            return;
                        _Removing=false;
                        i.Refresh();
                        i.Draw(gr);
                        switch (_Logika.GameEnd())
                        {
                            case Field.black:
                                MessageBox.Show("Fehér nyert.");
                                MenuFileNewGame_Click(null, null);
                                break;
                            case Field.white:
                                MessageBox.Show("Fekete nyert.");
                                MenuFileNewGame_Click(null, null);
                                break;
                            case Field.empty:
                                break;
                        }
                        return;
                    }

					_Logika.SwitchActivePlayer();

                    switch (_Logika.Phase)
                    {
                        case Logic.GamePhase.first:
                        if (!_Logika.Step(0, 0, i.LogicX, i.LogicY))
                           return;
                        if (_Logika.CheckMalom(i.LogicX, i.LogicY, _Logika.ActivePlayer))
                            _Removing = true;
                        break;

                        case Logic.GamePhase.second:
                        if (FromField == null)
                        {
                            if (i.Value != _Logika.ActivePlayer)
                            {
                                return;
                            }
                            FromField = i;
                        }
                        else
                        {
                            if (!_Logika.Step(FromField.LogicX, FromField.LogicY, i.LogicX, i.LogicY))
                            {
                                return;
                            }
                            if (_Logika.CheckMalom(i.LogicX, i.LogicY, _Logika.ActivePlayer))
                                _Removing = true;
                            FromField.Refresh();
                            FromField.Draw(gr);
                            FromField = null;
                        }
                        break;
                    }
                    i.Refresh();
                    i.Draw(gr);
                }
            }
        }

        // Menü eseményvezérlők
        private void MenuFileExit_Click(object sender, EventArgs e) { Close(); }

        private void MenuFileNewGame_Click(object sender, EventArgs e)
        {
            _Logika.NewGame();
            Graphics gr = boardpanel.CreateGraphics();
            foreach (GuiField i in _GuiFields)
            {
                i.Refresh();
                i.Draw(gr);
            }
        }

        private void MenuFileLoadGame_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _Logika.LoadGame(openFileDialog.FileName);
                    Graphics gr = boardpanel.CreateGraphics();
                    foreach (GuiField i in _GuiFields)
                    {
                        i.Refresh();
                        i.Draw(gr);
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show(
						"Játék betöltése sikertelen!" + 
						Environment.NewLine + 
						"Hibás az elérési út, vagy a fájlformátum.", 
						"Hiba!",
						MessageBoxButtons.OK, 
						MessageBoxIcon.Error);
                    _Logika.NewGame();
                }
                finally
                {
                }
            }
        }

        private void MenuFileSaveGame_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _Logika.SaveGame(saveFileDialog.FileName);
                }
                catch (Exception)
                {
                    MessageBox.Show(
						"Játék mentése sikertelen!" + 
						Environment.NewLine + 
						"Hibás az elérési út, " +
						"vagy a könyvtár nem írható.", 
						"Hiba!", 
						MessageBoxButtons.OK,
						MessageBoxIcon.Error);
                }
                finally
                {
                }
            }
        }

        private void Gui_Load(object sender, EventArgs e)
        {

        }
    }

    /// <summary>
    /// A grafikus mező osztály
    /// </summary>
    class GuiField
    {
        private Int32 _LogicX;
        private Int32 _LogicY;
        private Rectangle _GuiPos;
        private Field _Value;
        private Logic _Logic;

        public GuiField(Logic logic, Int32 x, Int32 y, Rectangle rectangle)
        {
            this._Logic = logic;
            this._LogicX = x;
            this._LogicY = y;
            this._GuiPos = rectangle;
            _Value = _Logic.TableValue(_LogicX, _LogicY);
        }

        /// <summary>
        /// Mező értékének lekérdezése
        /// </summary>
        public Field Value { get { return _Value; } }

        /// <summary>
        /// Logikai X pozició lekérdezése
        /// </summary>
        public Int32 LogicX { get { return _LogicX; } }

        /// <summary>
        /// Logikai Y pozició lekérdezése
        /// </summary>
        public Int32 LogicY { get { return _LogicY; } }

        /// <summary>
        /// Pozíció lekérdezése
        /// </summary>
        public Rectangle GuiPos { get { return _GuiPos; } }

        /// <summary>
        /// Újrakéri a logikától az őhozzá tartozó értéket.
        /// </summary>
        public void Refresh() { _Value = _Logic.TableValue(_LogicX, _LogicY); }

        /// <summary>
        /// Kirajzolás
        /// </summary>
        /// <param name="gr"></param>
        public void Draw(Graphics gr)
        {
            switch (_Value)
            {
                case Field.black:
                    gr.FillEllipse(Brushes.Black, _GuiPos);
                    break;
                case Field.white:
                    gr.FillEllipse(Brushes.LightGray, _GuiPos);
                    break;
                case Field.empty:
                    gr.FillEllipse(Brushes.White, _GuiPos);
                    gr.DrawEllipse(Pens.Black, _GuiPos);
                    break;
            }
        }
    }
}
