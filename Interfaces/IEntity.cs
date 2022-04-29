using UnityEngine;

/// <summary>
/// English: Base Interface for all entities
/// ���{��F������Entity�p�̃x�[�X�C���^�[�t�F�[�X
/// </summary>
public interface IEntity
{
    /// <summary>
    /// English: Either Selectable or UnSelectable
    /// ���{��F�I���ł��邩�ǂ���
    /// </summary>
    public enum SelectionType
    {
        Selectable,
        UnSelectable
    }
    /// <summary>
    /// English: Relationship with other entities in the game: Ally, Enemy, Neutral
    /// ���{��F����Entity�Ƃ̊֌W���F�����A�G�A����
    /// </summary>
    public enum RelationshipType
    {
        Ally,
        Enemy,
        Neutral
    }
    /// <summary>
    /// English�FEither Unit or Structure
    /// ���{��F���j�b�g���r���f�B���O��
    /// </summary>
    public enum EntityType
    {
        Unit,
        Structure
    }

    /// <summary>
    /// English: Return the selection type
    /// ���{��F�I���\�̃^�C�v��Ԃ�
    /// </summary>
    /// <returns></returns>
    public SelectionType GetSelectionType();
    /// <summary>
    /// English: Return the relationship type
    /// ���{��F�֌W����Ԃ�
    /// </summary>
    /// <returns></returns>
    public RelationshipType GetRelationshipType();
    /// <summary>
    /// English: Return the entity type
    /// ���{��FEntity�̃^�C�v��Ԃ�
    /// </summary>
    /// <returns></returns>
    public EntityType GetEntityType();

    /// <summary>
    /// English: Return the sprite for the portrait
    /// ���{��F�|�[�g���[�g��Sprite��Ԃ�
    /// </summary>
    /// <returns></returns>
    public Sprite GetPortrait();
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
    /// English: Return self (run-time type)
    /// ���{��F������Ԃ�(run-time�^�C�v�j
    /// </summary>
    /// <returns></returns>
    public dynamic GetSelf();
    /// <summary>
    /// English: Return its own (Unit/Structure)ActionController component (run-time type)
    /// ���{��F(Unit/Structure)�̍s���Ǘ��R���|�[�l���g��Ԃ��irun-time �^�C�v�j
    /// </summary>
    /// <returns></returns>
    public dynamic GetActionController();
    /// <summary>
    /// English: Return the type of itself (run-time type)
    /// ���{��F�����̃^�C�v��Ԃ��irun-time �^�C�v�j
    /// </summary>
    /// <returns></returns>
    public dynamic ReturnSelfType();
    /// <summary>
    /// English: Return a new empty action instance of depending on its info class (run-time type)
    /// ���{��F�����̃N���X�ɉ�����V�����A��̍s���̃C���X�^���X��Ԃ��irun-time �^�C�v�j
    /// </summary>
    /// <returns></returns>
    public dynamic ReturnNewAction();
    /// <summary>
    /// English: Return the type of its action class depending on its info class (run-time type)
    /// ���{��F�����̃N���X�ɉ�����s���̃N���X�̃^�C�v��Ԃ��irun-time �^�C�v�j
    /// </summary>
    /// <returns></returns>
    public dynamic ReturnActionType();

}
