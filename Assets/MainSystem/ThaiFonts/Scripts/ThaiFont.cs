using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ThaiFont : MonoBehaviour
{
    public List<TMP_FontAsset> fontAsset;

    void Start()
    {
        fontAsset.ForEach(o => { o.fontFeatureTable.glyphPairAdjustmentRecords.Clear(); });

            fontAsset.ForEach(o => {
            //ทิ่ ทิ้ ทิ๋ ทิ๊ ทิ๋ ทิ์ ที่ ที้ ที๋ ที๊ ทึ่ ทึ้ ทึ๋ ทึ๊ ทื่ ทื้ ทื๋ ทื๊ ทำ่ ทำ้ ท๋ำ ท๋ำ ท๊ำ ป่ ป้ป๋ ป๋ ป๊ ฝ่ ฝ้ ฝ๋ ฝ๊

            TMP_GlyphValueRecord valueZero = new TMP_GlyphValueRecord(0,0,0,0);
            TMP_GlyphValueRecord vowel_1 = new TMP_GlyphValueRecord(0, 10, 0, 0);
            TMP_GlyphValueRecord vowel_2 = new TMP_GlyphValueRecord(0, 20, 0, 0);
            TMP_GlyphValueRecord vowel_3 = new TMP_GlyphValueRecord(-30, 25, 0, 0);


            AddGlyphAdjustmentRecord(o, 738, 721, valueZero, vowel_1); //ทิ่
            AddGlyphAdjustmentRecord(o, 738, 724, valueZero, vowel_1); //ทิ้
            AddGlyphAdjustmentRecord(o, 738, 730, valueZero, vowel_1); //ทิ๋
            AddGlyphAdjustmentRecord(o, 738, 727, valueZero, vowel_1); //ทิ๊
            AddGlyphAdjustmentRecord(o, 738, 732, valueZero, vowel_1); //ทิ์

            AddGlyphAdjustmentRecord(o, 719, 721, valueZero, vowel_2); //ทั่
            AddGlyphAdjustmentRecord(o, 719, 724, valueZero, vowel_2); //ทั้
            AddGlyphAdjustmentRecord(o, 719, 730, valueZero, vowel_2); //ทั๋
            AddGlyphAdjustmentRecord(o, 719, 727, valueZero, vowel_2); //ทั๊
            AddGlyphAdjustmentRecord(o, 719, 732, valueZero, vowel_2); //ทั์

            AddGlyphAdjustmentRecord(o, 740, 721, valueZero, vowel_2); //ที่
            AddGlyphAdjustmentRecord(o, 740, 724, valueZero, vowel_2); //ที้
            AddGlyphAdjustmentRecord(o, 740, 730, valueZero, vowel_2); //ที๋
            AddGlyphAdjustmentRecord(o, 740, 727, valueZero, vowel_2); //ที๊
            AddGlyphAdjustmentRecord(o, 740, 732, valueZero, vowel_2); //ที์

            AddGlyphAdjustmentRecord(o, 742, 721, valueZero, vowel_2); //ทื่
            AddGlyphAdjustmentRecord(o, 742, 724, valueZero, vowel_2); //ทื้
            AddGlyphAdjustmentRecord(o, 742, 730, valueZero, vowel_2); //ทื๋
            AddGlyphAdjustmentRecord(o, 742, 727, valueZero, vowel_2); //ทื๊
            AddGlyphAdjustmentRecord(o, 742, 732, valueZero, vowel_2); //ทื์

            AddGlyphAdjustmentRecord(o, 744, 721, valueZero, vowel_2); //ทึ่
            AddGlyphAdjustmentRecord(o, 744, 724, valueZero, vowel_2); //ทึ้
            AddGlyphAdjustmentRecord(o, 744, 730, valueZero, vowel_2); //ทึ๋
            AddGlyphAdjustmentRecord(o, 744, 727, valueZero, vowel_2); //ทึึ๊
            AddGlyphAdjustmentRecord(o, 744, 732, valueZero, vowel_2); //ทื์

            AddGlyphAdjustmentRecord(o, 484, 721, valueZero, vowel_3); //ท่ำ
            AddGlyphAdjustmentRecord(o, 484, 724, valueZero, vowel_3); //ท้ำ
            AddGlyphAdjustmentRecord(o, 484, 730, valueZero, vowel_3); //ท๋ำ
            AddGlyphAdjustmentRecord(o, 484, 727, valueZero, vowel_3); //ท๊ำ


            AddGlyphAdjustmentRecord(o, 721, 484, valueZero, vowel_3); //ท่ำ
            AddGlyphAdjustmentRecord(o, 724, 484, valueZero, vowel_3); //ท้ำ
            AddGlyphAdjustmentRecord(o, 730, 484, valueZero, vowel_3); //ท๋ำ
            AddGlyphAdjustmentRecord(o, 727, 484, valueZero, vowel_3); //ท๊ำ

            // บันทึกการเปลี่ยนแปลงในฟอนต์
            TMPro_EventManager.ON_FONT_PROPERTY_CHANGED(true, o);
        });
    }

    void AddGlyphAdjustmentRecord(TMP_FontAsset _fontAsset, uint _firstGlyph, uint _secondGlyph, TMP_GlyphValueRecord _firstGlyphOffset, TMP_GlyphValueRecord _secondGlyphOffset)
    {
        TMP_GlyphAdjustmentRecord firstAdjustmentRecord = new TMP_GlyphAdjustmentRecord(_firstGlyph, _firstGlyphOffset);
        TMP_GlyphAdjustmentRecord secondAdjustmentRecord = new TMP_GlyphAdjustmentRecord(_secondGlyph, _secondGlyphOffset);

        TMP_GlyphPairAdjustmentRecord glyphPairAdjustmentRecord = new TMP_GlyphPairAdjustmentRecord(firstAdjustmentRecord, secondAdjustmentRecord);

        _fontAsset.fontFeatureTable.glyphPairAdjustmentRecords.Add(glyphPairAdjustmentRecord);
    }
}
