using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Mygame;

namespace Com.Mygame
{
    public enum State { START, END, SEMOV, ESMOV, WIN, LOSE };
    
    public interface IUserActions
    {
        void angelSOnB();
        void angelEOnB();
        void devilSOnB();
        void devilEOnB();
        void moveBoat();
        void offBoatL();
        void offBoatR();
        void restart();
    }

    public class GameSceneController : System.Object, IUserActions
    {

        private static GameSceneController _instance;
        private BaseCode _baseCode;
        private GenGameObject _ggo;
        public State state = State.START;

        public static GameSceneController GetInstance()
        {
            if (null == _instance)
            {
                _instance = new GameSceneController();
            }
            return _instance;
        }

        public BaseCode getBaseCode()
        {
            return _baseCode;
        }

        internal void setBaseCode(BaseCode bc)
        {
            if (null == _baseCode)
            {
                _baseCode = bc;
            }
        }

        public GenGameObject getGenGameObject()
        {
            return _ggo;
        }

        internal void setGenGameObject(GenGameObject ggo)
        {
            if (null == _ggo)
            {
                _ggo = ggo;
            }
        }

        public void angelSOnB()
        {
            _ggo.AngelStartOnBoat();
        }

        public void angelEOnB()
        {
            _ggo.AngelEndOnBoat();
        }

        public void devilSOnB()
        {
            _ggo.DevilStartOnBoat();
        }

        public void devilEOnB()
        {
            _ggo.DevilEndOnBoat();
        }

        public void moveBoat()
        {
            _ggo.moveBoat();
        }

        public void offBoatL()
        {
            _ggo.getOffTheBoat(0);
        }

        public void offBoatR()
        {
            _ggo.getOffTheBoat(1);
        }

        public void restart()
        {
            Application.LoadLevel(Application.loadedLevelName);
            state = State.START;
        }
    }
}

public class BaseCode : MonoBehaviour
{

    public string gameName;
    public string gameRule;

    void Start()
    {
        GameSceneController my = GameSceneController.GetInstance();
        my.setBaseCode(this);
        gameName = "Priests and Devils";
        gameRule = "Priests and Devils is a puzzle game in which you will help the Priests and Devils to cross the river within the time limit. There are 3 priests and 3 devils at one side of the river. They all want to get to the other side of this river, but there is only one boat and this boat can only carry two persons each time. And there must be one person steering the boat from one side to the other side. In the flash game, you can click on them to move them and click the go button to move the boat to the other direction. If the priests are out numbered by the devils on either side of the river, they get killed and the game is over. You can try it in many ways. Keep all priests alive! Good luck!             Sphere -- Priest    Cube -- Devil";
    }
}