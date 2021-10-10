using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessengerBotManager
{
    class BraceFoldingStrategy
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////// Property
        ////////////////////////////////////////////////////////////////////////////////////////// Public

        #region 개방 중괄호 - OpeningBrace

        /// <summary>
        /// 개방 중괄호
        /// </summary>
        public char OpeningBrace { get; set; }

        #endregion
        #region 폐쇄 중괄호 - ClosingBrace

        /// <summary>
        /// 폐쇄 중괄호
        /// </summary>
        public char ClosingBrace { get; set; }

        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////// Constructor
        ////////////////////////////////////////////////////////////////////////////////////////// Public

        #region 생성자 - BraceFoldingStrategy()

        /// <summary>
        /// 생성자
        /// </summary>
        public BraceFoldingStrategy()
        {
            OpeningBrace = '{';
            ClosingBrace = '}';
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////// Method
        ////////////////////////////////////////////////////////////////////////////////////////// Public

        #region 새 폴딩 생성하기 - CreateNewFoldings(source)

        /// <summary>
        /// 새 폴딩 생성하기
        /// </summary>
        /// <param name="source">텍스트 소스</param>
        /// <returns>새 폴딩 열거 가능형</returns>
        public IEnumerable<NewFolding> CreateNewFoldings(ITextSource source)
        {
            List<NewFolding> newFoldingList = new List<NewFolding>();
            Stack<int> offsetStack = new Stack<int>();
            int lastNewLineOffset = 0;
            char openingBrace = OpeningBrace;
            char closingBrace = ClosingBrace;

            for (int i = 0; i < source.TextLength; i++)
            {
                char character = source.GetCharAt(i);

                if (character == openingBrace)
                {
                    offsetStack.Push(i);
                }
                else if (character == closingBrace && offsetStack.Count > 0)
                {
                    int startOffset = offsetStack.Pop();

                    if (startOffset < lastNewLineOffset)
                    {
                        newFoldingList.Add(new NewFolding(startOffset, i + 1));
                    }
                }
                else if (character == '\n' || character == '\r')
                {
                    lastNewLineOffset = i + 1;
                }
            }

            newFoldingList.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));

            return newFoldingList;
        }

        #endregion
        #region 새 폴딩 생성하기 - CreateNewFoldings(document, firstErrorOffset)

        /// <summary>
        /// 새 폴딩 생성하기
        /// </summary>
        /// <param name="document">텍스트 문서</param>
        /// <param name="firstErrorOffset">첫번째 에러 오프셋</param>
        /// <returns>새 폴딩 열거 가능형</returns>
        public IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
        {
            firstErrorOffset = -1;

            return CreateNewFoldings(document);
        }

        #endregion
        #region 폴딩 업데이트하기 - UpdateFoldings(manager, document)

        /// <summary>
        /// 폴딩 업데이트하기
        /// </summary>
        /// <param name="manager">폴딩 관리자</param>
        /// <param name="document">텍스트 문서</param>
        public void UpdateFoldings(FoldingManager manager, TextDocument document)
        {
            int firstErrorOffset;

            IEnumerable<NewFolding> newFoldingEnumerable = CreateNewFoldings(document, out firstErrorOffset);

            manager.UpdateFoldings(newFoldingEnumerable, firstErrorOffset);
        }

        #endregion
    }
}
