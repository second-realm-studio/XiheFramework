using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class ImageSwitcherMixerBehaviour : PlayableBehaviour {
    Sprite m_DefaultSprite;

    Image m_TrackBinding;

    bool m_FirstFrameHappened;

    // private Texture2D tex1 = null;
    // private Texture2D tex2 = null;
    // private Texture2D mixed = null;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData) {
        m_TrackBinding = playerData as Image;

        if (m_TrackBinding == null)
            return;

        if (!m_FirstFrameHappened) {
            m_DefaultSprite = m_TrackBinding.sprite;
            m_FirstFrameHappened = true;
        }

        int inputCount = playable.GetInputCount();

        float totalWeight = 0f;
        float greatestWeight = 0f;
        // float secondGreatestWeight = 0f;
        int currentInputs = 0;

        for (int i = 0; i < inputCount; i++) {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<ImageSwitcherBehaviour> inputPlayable = (ScriptPlayable<ImageSwitcherBehaviour>) playable.GetInput(i);
            ImageSwitcherBehaviour input = inputPlayable.GetBehaviour();

            totalWeight += inputWeight;

            if (inputWeight>greatestWeight) {
                m_TrackBinding.sprite = input.image;
                greatestWeight = inputWeight;
            }
            
            // if (inputWeight > secondGreatestWeight) {
            //     if (inputWeight > greatestWeight) {
            //         tex1 = input.image.texture;
            //         greatestWeight = inputWeight;
            //     }
            //     else {
            //         tex2 = input.image.texture;
            //         secondGreatestWeight = inputWeight;
            //     }
            //
            //     if (tex2 == null) {
            //         m_TrackBinding.sprite = input.image;
            //     }
            //     else {
            //         m_TrackBinding.sprite = MergeTextures(tex1, tex2);
            //     }
            // }


            if (!Mathf.Approximately(inputWeight, 0f))
                currentInputs++;
        }

        if (currentInputs != 1 && 1f - totalWeight > greatestWeight) {
            m_TrackBinding.sprite = m_DefaultSprite;
        }
    }

    // private Sprite MergeTextures(Texture2D img, Texture2D overlay) {
    //     // int width = img.width > overlay.width ? img.width : overlay.width;
    //     // int height = img.height > overlay.height ? img.height : overlay.height;
    //     // Texture2D result=new Texture2D(,height);
    //     
    //     Color[] cols1 = img.GetPixels();
    //     Color[] cols2 = overlay.GetPixels();
    //     for (var i = 0; i < cols1.Length; ++i) {
    //         float rOut = (cols2[i].r * cols2[i].a) + (cols1[i].r * (1 - cols2[i].a));
    //         float gOut = (cols2[i].g * cols2[i].a) + (cols1[i].g * (1 - cols2[i].a));
    //         float bOut = (cols2[i].b * cols2[i].a) + (cols1[i].b * (1 - cols2[i].a));
    //         float aOut = cols2[i].a + (cols1[i].a * (1 - cols2[i].a));
    //
    //         cols1[i] = new Color(rOut, gOut, bOut, aOut);
    //     }
    //
    //     Texture2D result=new Texture2D(img.width,img.height);
    //     //Graphics.CopyTexture(overlay, result);
    //     result.SetPixels(cols1);
    //     result.Apply();
    //
    //     var sprite = Sprite.Create(result, new Rect(0, 0, result.width, result.height), Vector2.zero);
    //     return sprite;
    // }

    public override void OnPlayableDestroy(Playable playable) {
        m_FirstFrameHappened = false;

        if (m_TrackBinding == null)
            return;

        m_TrackBinding.sprite = m_DefaultSprite;
    }
}