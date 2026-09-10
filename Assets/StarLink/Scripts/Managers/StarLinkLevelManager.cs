using UnityEngine;
using TMKOC;
using DG.Tweening;

namespace TMKOC.StarLink
{
    public class StarLinkLevelManager : LevelManager
    {

        public RectTransform m_LevelTextConatiner;
        protected override void Start()
        {
            base.Start();
        }

        public override void LoadLevel(int levelIndex)
        {
            base.LoadLevel(levelIndex);

            m_LevelTextConatiner.DOKill();

            Sequence seq = DOTween.Sequence();

            seq.Append(m_LevelTextConatiner.DOLocalMoveY(0f, 1f)
                .SetEase(Ease.OutCubic));

            seq.AppendInterval(3f);

            seq.Append(m_LevelTextConatiner.DOLocalMoveY(500f, 1f)
                .SetEase(Ease.InCubic));
        }
    }
}
