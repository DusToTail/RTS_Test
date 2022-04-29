using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// English: a base class for upgrades. Will use events to inform all relevant entities (listeners) to update their stats or unlock abilities.
/// ���{��F�A�b�v�O���[�h�̃x�[�X�N���X�B�C�x���g�Ŋ֌W�̂���Entity�i���X�i�[�j�����l���X�V������A�r���e�B������������ł���悤���M����B
/// </summary>
public class Upgrade
{
    public string name;
    public string description;
    public Sprite image;


    public enum Type
    {
        Stats,
        Unlock
    }
    public Type type;


}
