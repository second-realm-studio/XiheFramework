using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;
using XiheFramework;

/// <summary>
/// calculate and record beat/input info
/// </summary>
public class BeatModule : GameModule {
    public int bpm = 90;

    //public int signature = 4; //4/4 3/4 etc
    public int delay = 4;
    //public float inputBlur = 0.1f;

    private bool m_Active = false;

    //private int[] m_InputBuffer;
    //private int m_InputBuffer;
    private int m_CurrentBeat = 0;

    private float m_Timer = 0f;

    private float m_CumulativeTimer = 0f;
    //private bool m_WaitingInput = false;

    public float TimePerBeat => 60f / bpm; //time per beat
    public float Timer => m_Timer;
    public float CumulativeTimer => m_CumulativeTimer;

    // public float MaxInputBlur => TimePerBeat; //time per beat

    // public bool IsWaitingInput() {
    //     return m_WaitingInput;
    // }

    private void Start() {
        Game.Event.Subscribe("OnPlay", OnPlay);
    }

    private void OnPlay(object sender, object e) {
        m_Timer = -delay * TimePerBeat;
        m_CumulativeTimer = -delay * TimePerBeat;
        m_CurrentBeat = -delay;

        m_Active = true;
    }

    // private void ResetInputBuffer() {
    //     // m_InputBuffer = new int[signature];
    //     //m_InputBuffer = 0;
    // }

    // void ProcessInput() {
    //     if (!m_WaitingInput) {
    //         return;
    //     }
    //
    //     // if (Game.Input.GetKeyDown(KeyActionTypes.Up)) {
    //     //     //m_InputBuffer[m_CurrentBeat] = 1;
    //     //     m_InputBuffer = 1;
    //     //     m_WaitingInput = false;
    //     // }
    //
    //     if (Game.Input.GetKeyDown("Left")) {
    //         // m_InputBuffer[m_CurrentBeat] = 2;
    //         m_InputBuffer = 1;
    //         m_WaitingInput = false;
    //     }
    //
    //     // if (Game.Input.GetKeyDown(KeyActionTypes.Down)) {
    //     //     // m_InputBuffer[m_CurrentBeat] = 3;
    //     //     m_InputBuffer = 3;
    //     //     m_WaitingInput = false;
    //     // }
    //
    //     if (Game.Input.GetKeyDown("Right")) {
    //         // m_InputBuffer[m_CurrentBeat] = 4;
    //         m_InputBuffer = 2;
    //         m_WaitingInput = false;
    //     }
    // }

    // void SendBufferInfo() {
    //     Game.Event.Invoke("OnBeatInput", this, m_InputBuffer);
    //     Debug.Log("Input :" + m_InputBuffer);
    //     //BeatInputData inputData = new BeatInputData {inputDirection = m_InputBuffer};
    //     //Game.Event.Invoke("OnPlayerBarEnd", this, inputData);
    // }

    // void UpdateBeat() {
    //     m_Timer += Time.deltaTime;
    //     if (m_Timer < 60f / (float) bpm) {
    //         return;
    //     }
    //
    //     // //no input this beat
    //     // if (m_WaitingInput) {
    //     //     // m_InputBuffer[m_CurrentBeat] = 0;
    //     //     m_InputBuffer = 0;
    //     // }
    //
    //     m_CurrentBeat = (m_CurrentBeat + 1);
    //     Game.Blackboard.SetData("BeatCount", m_CurrentBeat, BlackBoardDataType.Runtime);
    //     Game.Event.Invoke("OnBeat", this, m_CurrentBeat);
    //
    //     //send buffer and clear
    //     //SendBufferInfo();
    //     //ResetInputBuffer();
    //
    //     m_Timer -= (60f / (float) bpm);
    // }

    public override void Update() {
        if (!m_Active) {
            return;
        }

        m_Timer += Time.deltaTime;
        m_CumulativeTimer += Time.deltaTime;
        if (m_Timer < TimePerBeat) {
            return;
        }

        m_CurrentBeat = (m_CurrentBeat + 1);
        Game.Blackboard.SetData("BeatCount", m_CurrentBeat, BlackBoardDataType.Runtime);
        //Game.Event.Invoke("OnBeat", this, m_CurrentBeat);

        m_Timer -= TimePerBeat;

        //inputBlur = Mathf.Clamp(inputBlur, 0f, 30f / bpm);

        // if (m_Timer <= inputBlur / 2f || m_Timer > 60f / bpm - inputBlur / 2f) {
        //     m_WaitingInput = true;
        // }
        // else {
        //     m_WaitingInput = false;
        // }

        //accept input in a range instead of every/one frame
        // if (Mathf.Abs(m_Timer) > inputBlur) {
        //     m_WaitingInput = false;
        // }
        // else {
        //     m_WaitingInput = true;
        // }

        //ProcessInput();
    }

    public override void ShutDown(ShutDownType shutDownType) {
        m_Timer = 0f;
        m_CumulativeTimer = 0f;
        m_CurrentBeat = 0;
    }
}