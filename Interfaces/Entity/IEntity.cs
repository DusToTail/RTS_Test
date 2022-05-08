using UnityEngine;

/// <summary>
/// English: Base Interface for all entities
/// ���{��F������Entity�p�̃x�[�X�C���^�[�t�F�[�X
/// </summary>
public interface IEntity
{
    public enum Type
    {
        Structure,
        Unit
    }

    /// <summary>
    /// English: Return true if selectable by mouse. else false
    /// ���{��F�}�E�X�őI�ׂ����true��Ԃ��B�����łȂ��ƁAfalse��Ԃ�
    /// </summary>
    /// <returns></returns>
    public bool IsSelectable();

    /// <summary>
    /// English: Turn on/off the selected circle when selected
    /// ���{��F�I�΂ꂽ���ɁA�~���I���E�I�t�ɂ���
    /// </summary>
    /// <param name="isOn"></param>
    public void RenderSelectedCircle(bool _isOn);

    /// <summary>
    /// English: Return name
    /// ���{��F���O��Ԃ�
    /// </summary>
    /// <returns></returns>
    public string GetName();
    /// <summary>
    /// English: Return description
    /// ���{��F�f�B�X�N���v�V������Ԃ�
    /// </summary>
    /// <returns></returns>
    public string GetDescription();
    /// <summary>
    /// English: Return the sprite for the portrait
    /// ���{��F�|�[�g���[�g��Sprite��Ԃ�
    /// </summary>
    /// <returns></returns>
    public Sprite GetPortrait();
    /// <summary>
    /// English: Return the index of the player this entity belongs to
    /// ���{��F��������v���C���[�̃C���f�b�N�X��Ԃ��B
    /// </summary>
    /// <returns></returns>
    public int GetPlayerIndex();
    /// <summary>
    /// English: Return the type of this entity
    /// ���{��FEntity�̃^�C�v��Ԃ��B
    /// </summary>
    /// <returns></returns>
    public Type GetEntityType();
    /// <summary>
    /// English: Return the selected circle
    /// ���{��F�I�΂ꂽ���̉~�̕Ԃ�
    /// </summary>
    /// <returns></returns>
    public GameObject GetSelectedCircle();
    /// <summary>
    /// English: Return the selected circle radius
    /// ���{��F�I�΂ꂽ���̉~�̔��a��Ԃ�
    /// </summary>
    /// <returns></returns>
    public float GetSelectedCircleRadius();

    /// <summary>
    /// English: Return the Transform
    /// ���{��FTransform��Ԃ�
    /// </summary>
    /// <returns></returns>
    public Transform GetTransform();
    /// <summary>
    /// English: Return the current world position
    /// ���{��F���݂�World�ʒu��Ԃ�
    /// </summary>
    /// <returns></returns>
    public Vector3 GetWorldPosition();

    /// <summary>
    /// English: Return the class implementing this interface
    /// ���{��F���̃C���^�[�t�F�[�X����������N���X��Ԃ�
    /// </summary>
    /// <returns></returns>
    public EntityInfo GetSelf();

}
