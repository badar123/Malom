/// Badics Áron Gergely - F846WA

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Malom
{
    /// <summary>
    /// A pálya mezőjét ez a 3 lehetséges enum érték határozza meg.
    /// </summary>
    public enum Field { empty, white, black }

    /// <summary>
    /// A táblát reprezentáló osztály.
    /// </summary>
    class Table
    {

        /// 
        /// Az alábbi ekvivalens formában tároljuk a táblát:
        /// 
        /// a------b------c
        /// |      |      |
        /// | d----e----f |
        /// | |    |    | |              \    *-b-c-o-x-w-v-j-a-*
        /// | | g--h--i | |        =======\     |   |   |   |
        /// | | |     | | |        =======/   *-e-f-n-u-t-s-k-d-*
        /// j-k-l     m-n-o              /      |   |   |   |
        /// | | |     | | |                   *-h-i-m-r-q-p-l-g-*
        /// | | p--q--r | |
        /// | |    |    | |
        /// | s----t----u |
        /// |      |      |
        /// v------w------x
        ///

        private const Int32 _MAP_XSIZE = 8;
        private const Int32 _MAP_YSIZE = 3;
        private Field[,] _Map = new Field[_MAP_XSIZE, _MAP_YSIZE];

        public Int32 MAP_XSIZE { get { return _MAP_XSIZE; } }
        public Int32 MAP_YSIZE { get { return _MAP_YSIZE; } }

        public Table() { Reset(); }

        /// <summary>
        /// Mező lekérdezése
        /// 
        /// A későbbi könnyebb kezelhetőség miatt túlcímzés esetén "túlcsordul" a tömb.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Field this[Int32 x, Int32 y]
        {
            get 
            {
                return _Map[
                    x<0 ? _MAP_XSIZE + x : x % _MAP_XSIZE,
                    y<0 ? _MAP_YSIZE + y : y % _MAP_YSIZE
                    ]; 
            }
            set 
            { 
                _Map[
                    x<0 ? _MAP_XSIZE + x : x % _MAP_XSIZE,
                    y<0 ? _MAP_XSIZE + y : y % _MAP_YSIZE
                    ] = value; 
            }
        }

        /// <summary>
        /// Pálya kiűrítése
        /// </summary>
        public void Reset()
        {
            for (int i = 0; i < _MAP_XSIZE; ++i)
                for (int j = 0; j < _MAP_YSIZE; ++j)
                    _Map[i, j] = Field.empty;
        }
    }

    /// <summary>
    /// Logika osztály
    /// </summary>
    public partial class Logic
    {
        /// <summary>
        /// Játék fázisok:
        /// first = Korongok lerakásának fázisa.
        /// second = Első fázis utáni játék.
        /// </summary>
        public enum GamePhase { first, second }

        private GamePhase _Phase = GamePhase.first;
        private Field _ActivePlayer = Field.white;
        private const Int32 _MAX_PIECES = 9;
        private Int32 _WhitePieces = 0;
        private Int32 _BlackPieces = 0;
        private Int32 _FirstPhaseRound = 0;
        private Table _Table = new Table();

        /// <summary>
        /// Játék fázisának lekérdezése
        /// </summary>
        public GamePhase Phase { get { return _Phase; } }

        /// <summary>
        /// Akcióban lévő játékos lekérdezése
        /// </summary>
        public Field ActivePlayer { get { return _ActivePlayer; } }

        /// <summary>
        /// Adott koordinátához tartozó érték lekérdezése.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Field TableValue(Int32 x, Int32 y) { return _Table[x, y]; }

        /// <summary>
        /// Elvesz vagy hozzáad egy játékos korongjaihoz.
        /// </summary>
        /// <param name="who">Kinek a korongjaihoz</param>
        /// <param name="wich">
        /// true - hozzáad egyet
        /// false - elvesz egyet
        /// </param>
        private void AddOrRemovePiece(Field who,bool wich)
        {
            switch (who)
            {
                case Field.black:
                    _BlackPieces = wich ? _BlackPieces + 1 : _BlackPieces - 1;
                    break;
                case Field.white:
                    _WhitePieces = wich ? _WhitePieces + 1 : _WhitePieces - 1;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Új játék indítása
        /// </summary>
        public void NewGame()
        {
            _Table.Reset();
            _WhitePieces = _BlackPieces = _FirstPhaseRound = 0;
            _Phase = GamePhase.first;
            _ActivePlayer = Field.white;
        }

        /// <summary>
        /// Játék mentése
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveGame(string fileName)
        {
            StreamWriter writer = new StreamWriter(fileName);
            char tmp='e';
            for (int i = 0; i < _Table.MAP_XSIZE; ++i)
            {
                for (int j = 0; j < _Table.MAP_YSIZE; ++j)
                {
                    switch (_Table[i, j])
                    {
                        case Field.black: tmp = 'b'; break;
                        case Field.white: tmp = 'w'; break;
                        default: tmp = 'e'; break;
                    }
                    writer.Write(tmp);
                }
                writer.WriteLine();
            }
            switch (_ActivePlayer)
            {
                case Field.black: tmp = 'b'; break;
                case Field.white: tmp = 'w'; break;
                default: tmp = 'w'; break;
            }
            writer.Write(tmp);
            writer.WriteLine();
            writer.Close();
        }

        /// <summary>
        /// Játék betöltése
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadGame(string fileName)
        {
            StreamReader reader = new StreamReader(fileName);
            string tmpstr = "";
            for (int i = 0; i < _Table.MAP_XSIZE; ++i)
            {
                tmpstr = reader.ReadLine();
                for (int j = 0; j < _Table.MAP_YSIZE; ++j)
                {
                    switch (tmpstr[j])
                    {
                        case 'b': _Table[i, j] = Field.black; break;
                        case 'w': _Table[i, j] = Field.white; break;
                        default: _Table[i, j] = Field.empty; break;
                    }
                }
            }
            tmpstr = reader.ReadLine();
            switch (tmpstr[0])
            {
                case 'b': _ActivePlayer = Field.black; break;
                default: _ActivePlayer = Field.white; break;
            }
            reader.Close();
        }

		/// <summary>
        /// Aktív játékos váltása
        /// </summary>
		public void SwitchActivePlayer ()
		{
			_ActivePlayer = _ActivePlayer == Field.white ? Field.black : Field.white;
		}

        /// <summary>
        /// Korong lerakása vagy mozgatása.
        /// Nem legális lépés esetén nem történik meg a lépés.
        /// </summary>
        /// <param name="fromX"></param>
        /// <param name="fromY"></param>
        /// <param name="toX"></param>
        /// <param name="toY"></param>
        /// <returns> Legális-e a lépés. </returns>
        public bool Step(Int32 fromX, Int32 fromY, Int32 toX, Int32 toY)
        {
            if (_Table[toX, toY] != Field.empty || (_Table[fromX,fromY] != _ActivePlayer && _Phase!=GamePhase.first))
                return false;
            switch (_Phase)
            {
                case GamePhase.first:
                    _Table[toX,toY] = _ActivePlayer;
                    AddOrRemovePiece(_ActivePlayer, true);
                    _FirstPhaseRound++;

                    // Ha mindenki letette a korongjait, akkor kezdődik a második fázis.
                    if (_FirstPhaseRound==_MAX_PIECES*2)  
                        _Phase = GamePhase.second;

                    return true;

                case GamePhase.second:
 
                    // Ezt az egészet lehetne egy feltétellel is, de így valamivel átláthatóbb szerintem
                    if ((_ActivePlayer == Field.black && _BlackPieces > 3) || (_ActivePlayer == Field.white && _WhitePieces > 3))
                    {
                        if (fromY - toY == 0)
                        {
                            if (Math.Abs(fromX - toX) != 1)
                                if (Math.Abs(fromX - toX) != 7)
                                    return false;
                        }
                        else
                        {
                            if (fromX % 2 != 0 || fromX - toX != 0 || Math.Abs(fromY - toY) != 1)
                                return false;
                        }
                    }
                    break;
            }
            _Table[fromX, fromY] = Field.empty;
            _Table[toX, toY] = _ActivePlayer;
            return true;
        }

        /// <summary>
        /// Megnézi, hogy a korong malomba van-e.
        /// </summary>g
        /// <param name="stepX"></param>
        /// <param name="stepY"></param>
        /// <param name="who"> Fekete vagy fehér korong</param>
        /// <returns>
        /// true - malomban van az adott korong.
        /// false - nincs malomban.
        /// </returns>
        public bool CheckMalom(Int32 stepX, Int32 stepY, Field who)
        {
            if (stepX % 2 == 0) // középső esetén a két oldalsót ellenőrizzük.
            {
                if (
                       (_Table[stepX, stepY - 1] == who && _Table[stepX, stepY + 1] == who) ||
                       (_Table[stepX - 1, stepY] == who && _Table[stepX + 1, stepY] == who)
                   )
                   return true;
            }
            else // sarkon lévő esetén a két irányban lévőket.
            {
                if (
                       (_Table[stepX + 1, stepY] == who && _Table[stepX + 2, stepY] == who) ||
                       (_Table[stepX - 1, stepY] == who && _Table[stepX - 2, stepY] == who)
                   )
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Korong levétele a pályáról.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="who">Ki veszi le.</param>
        /// <returns>Sikeresen levettük-e.</returns>
        public bool RemovePiece(Int32 x, Int32 y, Field who)
        {
            Field field = _Table[x, y];
            if (field == Field.empty || field == who)
                return false;
            Field enemy = who == Field.black ? Field.white : Field.black;

            //Ha malomba van a korong, amit le akarunk venni, akkor ellenőrizni kell hogy az összes többi
            //korongja az ellenfélnek nincs-e malomban, mert ekkor a levétel legális. Mihelyst talál egyet, ami nincs,
            //a levétel érvénytelen.
            if (CheckMalom(x, y, field))
                for (int i = 0; i < _Table.MAP_XSIZE; ++i)
                    for (int j = 0; j < _Table.MAP_YSIZE; ++j)
                        if (_Table[i, j] == enemy && !CheckMalom(i, j, enemy))
                            return false;

            _Table[x, y] = Field.empty;
            AddOrRemovePiece(enemy, false);
            return true;
        }

        /// <summary>
        /// Játék végének ellenőrzése.
        /// </summary>
        /// <returns>
        /// black - fekete játékos nyert
        /// white - fehér játékos nyert
        /// empty - még nem nyert senki
        /// </returns>
        public Field GameEnd() 
        {
            if (Phase != GamePhase.first)
            {
                if (_BlackPieces < 3)
                    return Field.black;
                else if (_WhitePieces < 3)
                    return Field.white;
            }
            return Field.empty;
        }
    }
}
