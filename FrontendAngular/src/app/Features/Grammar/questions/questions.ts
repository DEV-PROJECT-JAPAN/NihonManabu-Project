// practice.component.ts
import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { GrammarClientService } from '../../../Core/Services/grammar-client-service';
import { GrammarModel } from '../../../Models/grammar-model';
import { QuestionModel } from '../../../Models/question-model';
import { AnswerModel } from '../../../Models/answer-model';
import { QuestionClientService } from '../../../Core/Services/question-client-service';

@Component({
  selector: 'app-grammar-questions', // Tên thẻ HTML đại diện cho component này (<app-grammar-questions>)
  standalone: true,                  // Định nghĩa đây là Standalone Component (không cần khai báo trong AppModule)
  imports: [CommonModule, RouterModule], // Nhập các module cơ bản và định tuyến của Angular
  providers: [GrammarClientService, QuestionClientService], // Khai báo các Service cung cấp dữ liệu cục bộ cho component này
  templateUrl: './questions.html',   // Đường dẫn tới file giao diện HTML
  styleUrls: ['./questions.css']     // Đường dẫn tới file định dạng giao diện CSS
})
export class Questions implements OnInit {
  // ==========================================
  // 1. CÁC BIẾN LƯU TRỮ ID TỪ ĐƯỜNG DẪN (URL)
  // ==========================================
  levelId!: number;   // Dùng để lưu ID của cấp độ học (N5, N4, N3...)
  lessonId!: number;  // Dùng để lưu ID của bài học cụ thể
  grammarId!: number; // Dùng để lưu ID của cấu trúc ngữ pháp đang luyện tập

  // ==========================================
  // 2. CÁC BIẾN QUẢN LÝ DỮ LIỆU CÂU HỎI
  // ==========================================
  questionType: number = -1; // Lưu dạng bài hiện tại. Mặc định là -1 (chưa chọn dạng nào)
  currentGrammar: GrammarModel | null = null; // Lưu chi tiết thông tin ngữ pháp (tên, định nghĩa, ví dụ...)
  questions: QuestionModel[] = []; // Mảng chứa toàn bộ danh sách câu hỏi tải về từ API

  // Object dạng Key-Value (Dùng ID câu hỏi làm Key để lấy ra mảng đáp án AnswerModel đã trộn)
  shuffledAnswersMap: { [questionId: number]: AnswerModel[] } = {};

  // ==========================================
  // 3. TRẠNG THÁI BÀI TẬP TRẮC NGHIỆM (QUIZ)
  // ==========================================
  quizEvaluated: { [questionId: number]: boolean } = {};      // Đánh dấu câu hỏi đã click chọn đáp án chưa (để khóa câu hỏi)
  quizSelectedAnswer: { [questionId: number]: AnswerModel } = {}; // Lưu dữ liệu đáp án mà người dùng đã click chọn
  quizCorrectAnswer: { [questionId: number]: AnswerModel } = {};  // Lưu dữ liệu đáp án đúng của câu đó để đối chiếu và tô màu xanh

  // ==========================================
  // 4. TRẠNG THÁI BÀI TẬP SẮP XẾP CÂU (ARRANGE)
  // ==========================================
  // ĐÃ SỬA: Thay đổi từ kiểu string[] thành mảng Object để lưu giữ vết poolIndex gốc dưới kho gợi ý
  arrangeAssembledWords: { [questionId: number]: { text: string, poolIndex: number }[] } = {};

  // Object hai tầng kiểm tra xem một chữ cụ thể ở kho gợi ý đã bị bấm chưa (để ẩn hoặc làm mờ chữ đó đi)
  arrangePoolStatus: { [questionId: number]: { [wordKey: string]: boolean } } = {};

  arrangeChecked: { [questionId: number]: boolean } = {};   // Đánh dấu xem người dùng đã bấm nút "Kiểm tra" câu sắp xếp này chưa
  arrangeIsCorrect: { [questionId: number]: boolean } = {}; // Kết quả kiểm tra câu sắp xếp của người dùng là Đúng (true) hay Sai (false)

  // Hàm khởi tạo, thực hiện Dependency Injection để nạp các dịch vụ (Services) cần dùng
  constructor(
    private _route: ActivatedRoute,            // Dịch vụ lấy thông tin từ URL hiện tại
    private _grammarService: GrammarClientService, // Dịch vụ gọi API liên quan tới Ngữ pháp
    private _questionService: QuestionClientService, // Dịch vụ gọi API liên quan tới Câu hỏi
    private _cdr: ChangeDetectorRef             // Dịch vụ quản lý cơ chế cập nhật/render lại giao diện của Angular
  ) { }

  /**
   * Hàm chạy tự động ngay sau khi Component được khởi tạo thành công
   */
  ngOnInit(): void {
    // Đăng ký (Subscribe) lắng nghe sự thay đổi của tham số trên URL
    this._route.params.subscribe(params => {
      // Dấu '+' đặt phía trước giúp ép kiểu giá trị từ dạng Chuỗi (String) trên URL thành dạng Số (Number)
      this.levelId = +params['levelId'];
      this.lessonId = +params['lessonId'];
      this.grammarId = +params['grammarId'];

      // Gọi API lấy thông tin chi tiết của ngữ pháp dựa trên grammarId
      this._grammarService.getGrammarByIdAsync(this.grammarId).subscribe(res => {
        this.currentGrammar = res; // Gán kết quả trả về từ API vào biến currentGrammar
        this._cdr.markForCheck();  // Báo cho Angular biết dữ liệu đã thay đổi, hãy cập nhật lại giao diện đi
      });
    });
  }

  /**
   * Xử lý khi người dùng chọn/đổi loại bài tập (Ví dụ: bấm nút Trắc nghiệm hoặc Sắp xếp câu)
   * @param type Mã số loại câu hỏi (Ví dụ: 0: Trắc nghiệm, 2: Sắp xếp)
   */
  changeQuestionType(type: number): void {
    this.questionType = type; // Cập nhật loại câu hỏi hiện tại sang loại mới được chọn
    if (type === -1) return;  // Nếu loại là -1 (chưa chọn gì hoặc mặc định) thì dừng hàm luôn, không làm gì tiếp

    // Gọi API lấy danh sách câu hỏi theo ID ngữ pháp và loại câu hỏi đã chọn
    this._questionService.getQuestionsByGrammarAsync(this.grammarId, this.questionType).subscribe(res => {

      // Tạo một bản sao hoàn toàn mới (Deep Clone) của dữ liệu API để tránh việc chỉnh sửa trực tiếp làm ảnh hưởng gốc
      let rawQuestions: QuestionModel[] = JSON.parse(JSON.stringify(res));

      // Nếu người dùng chọn dạng bài Trắc nghiệm (type === 1) hoặc Sắp xếp (type === 2), tiến hành xáo trộn thứ tự câu hỏi
      if (this.questionType === 1 || this.questionType === 2) {
        rawQuestions = this.shuffleArray(rawQuestions);
      }

      this.shuffledAnswersMap = {}; // Xóa trắng danh sách đáp án đã xáo trộn cũ

      // Duyệt qua từng câu hỏi vừa lấy về để xáo trộn các đáp án lựa chọn bên trong nó
      rawQuestions.forEach(q => {
        // Nếu câu hỏi có danh sách đáp án và mảng đáp án đó không rỗng
        this.shuffledAnswersMap[q.id] = q.answers && q.answers.length > 0
          ? this.shuffleArray(q.answers) // Tiến hành trộn ngẫu nhiên mảng đáp án của câu hỏi này
          : [];                          // Nếu không có đáp án thì gán mảng rỗng
      });

      this.questions = rawQuestions; // Gán toàn bộ danh sách câu hỏi đã chuẩn bị vào biến hiển thị trên giao diện
      this.resetExerciseStates();     // Gọi hàm reset lại toàn bộ trạng thái làm bài cũ về ban đầu
      this._cdr.markForCheck();       // Ép giao diện cập nhật để hiển thị bộ câu hỏi mới
    });
  }

  /**
   * Hàm bổ trợ: Trộn ngẫu nhiên các phần tử của một mảng (Thuật toán Fisher-Yates)
   * @param array Mảng đầu vào cần trộn
   */
  private shuffleArray<T>(array: T[]): T[] {
    const clone = [...array]; // Tạo một mảng clone chứa các phần tử giống mảng gốc để thao tác
    // Vòng lặp chạy lùi từ cuối mảng về đầu mảng
    for (let i = clone.length - 1; i > 0; i--) {
      const j = Math.floor(Math.random() * (i + 1)); // Lấy ngẫu nhiên một chỉ số j từ 0 đến i
      [clone[i], clone[j]] = [clone[j], clone[i]]; // Đổi chỗ vị trí của phần tử thứ i và phần tử thứ j cho nhau
    }
    return clone; // Trả về mảng mới đã được xáo trộn hoàn toàn ngẫu nhiên
  }

  /**
   * Đưa tất cả các biến theo dõi trạng thái làm bài của học viên về trạng thái trống rỗng
   */
  private resetExerciseStates(): void {
    this.quizEvaluated = {};        // Xóa lịch sử trạng thái khóa câu hỏi trắc nghiệm
    this.quizSelectedAnswer = {};   // Xóa lịch sử các đáp án trắc nghiệm đã chọn
    this.quizCorrectAnswer = {};    // Xóa lịch sử đáp án đúng trắc nghiệm
    this.arrangeAssembledWords = {}; // Xóa sạch các câu sắp xếp đang làm dở
    this.arrangePoolStatus = {};     // Mở khóa toàn bộ các chữ trong kho gợi ý sắp xếp
    this.arrangeChecked = {};        // Xóa trạng thái đã bấm nút "Kiểm tra" của bài sắp xếp
    this.arrangeIsCorrect = {};      // Xóa kết quả đúng/sai cũ của bài sắp xếp

    // Duyệt qua danh sách câu hỏi hiện tại để khởi tạo vùng dữ liệu trống cho riêng các câu dạng Sắp xếp (Type 2)
    this.questions.forEach(q => {
      if (q.questionType === 2) {
        this.arrangeAssembledWords[q.id] = []; // Khởi tạo mảng chữ rỗng cho câu này
        this.arrangePoolStatus[q.id] = {};     // Khởi tạo object quản lý kho chữ rỗng cho câu này
        this.arrangeChecked[q.id] = false;     // Đặt trạng thái chưa bấm nút kiểm tra cho câu này
        this.arrangeIsCorrect[q.id] = false;   // Đặt trạng thái kết quả mặc định là false
      }
    });
  }

  /**
   * Xử lý hành động khi học viên click chọn một đáp án trắc nghiệm
   * @param question Câu hỏi trắc nghiệm hiện tại
   * @param answer Đáp án mà học viên vừa click vào
   */
  public selectQuizAnswer(question: QuestionModel, answer: AnswerModel): void {
    if (this.quizEvaluated[question.id]) return; // Nếu câu hỏi này đã được click chọn trước đó rồi thì kết thúc hàm (khóa không cho chọn lại)

    this.quizEvaluated[question.id] = true; // Đánh dấu câu này đã làm, kích hoạt trạng thái khóa lựa chọn trên giao diện
    this.quizSelectedAnswer[question.id] = answer; // Lưu lại dữ liệu đáp án mà học viên chọn để tô màu xám/đỏ trên UI

    // Tìm kiếm trong danh sách đáp án gốc xem đáp án nào có trường dữ liệu isCorrect bằng true
    const correct = question.answers.find(a => a.isCorrect);
    if (correct) this.quizCorrectAnswer[question.id] = correct; // Lưu đáp án đúng tìm được vào biến để tô màu xanh trên UI
  }

  /**
   * Xử lý khi học viên click chọn một chữ từ "Kho chữ gợi ý" để đưa lên "Hàng câu kết quả" (Dạng bài sắp xếp)
   * @param questionId ID của câu hỏi sắp xếp hiện tại
   * @param wordText Nội dung chữ của từ vừa click (Ví dụ: "にほんご")
   * @param poolIndex Vị trí thứ tự của từ đó trong kho gợi ý
   */
  public handleWordClick(questionId: number, wordText: string, poolIndex: number): void {
    if (this.arrangeChecked[questionId]) return; // Nếu câu này học viên đã bấm nút "Kiểm tra" rồi thì khóa, không cho chọn thêm chữ

    const key = `${wordText}_${poolIndex}`; // Tạo ra một mã định danh duy nhất cho chữ này nhằm tránh trùng lặp nếu câu có 2 chữ giống nhau
    if (this.arrangePoolStatus[questionId][key]) return; // Nếu chữ có mã key này đã được bấm chọn trước đó rồi thì bỏ qua không xử lý

    this.arrangePoolStatus[questionId][key] = true; // Đánh dấu chữ này đã chọn = true (Giao diện dựa vào đây để ẩn/mờ chữ này đi ở kho)

    // ĐÃ SỬA: Đẩy cả chữ (text) và chỉ mục kho gốc (poolIndex) thành dạng Object vào mảng kết quả
    this.arrangeAssembledWords[questionId].push({ text: wordText, poolIndex: poolIndex });
  }

  /**
   * Xử lý khi học viên click vào một chữ nằm trên "Hàng câu kết quả" để hủy chọn nó (Đưa chữ đó quay trở lại kho gợi ý)
   * @param questionId ID của câu hỏi sắp xếp hiện tại
   * @param wordObj Đối tượng chữ chứa text và poolIndex gốc nhận từ HTML truyền vào
   * @param assembledIndex Vị trí thứ tự của từ đó trong hàng chữ kết quả học viên đang xếp
   */
  // ĐÃ SỬA: Thay tham số `wordText: string` thành đối tượng cấu trúc `wordObj: { text: string, poolIndex: number }`
  public removeAssembledWord(questionId: number, wordObj: { text: string, poolIndex: number }, assembledIndex: number): void {
    if (this.arrangeChecked[questionId]) return; // Nếu đã bấm nút "Kiểm tra" rồi thì không cho phép rút bớt chữ ra nữa

    this.arrangeAssembledWords[questionId].splice(assembledIndex, 1); // Xóa bỏ 1 phần tử tại vị trí assembledIndex khỏi hàng câu kết quả

    // ĐÃ SỬA: Tái tạo chính xác mã định danh bằng poolIndex gốc để mở khóa đúng ô chữ dưới kho gợi ý
    const key = `${wordObj.text}_${wordObj.poolIndex}`;
    this.arrangePoolStatus[questionId][key] = false; // Đặt trạng thái chữ này về false (Giao diện hiển thị sáng lại ở kho gợi ý)
  }

  /**
   * Xử lý khi bấm nút "Kiểm tra" để nộp và xem kết quả câu sắp xếp câu
   * @param question Đối tượng câu hỏi sắp xếp cần kiểm tra
   */
  public checkArrangeAnswer(question: QuestionModel): void {
    this.arrangeChecked[question.id] = true; // Đánh dấu câu hỏi này đã bấm nút nộp bài kiểm tra trên hệ thống

    const correctSentence = this.getCorrectSentenceText(question); // Gọi hàm bổ trợ để lấy ra chuỗi đáp án chuẩn xác từ database

    // ĐÃ SỬA: Do arrangeAssembledWords giờ chứa mảng Object, cần qua .map(w => w.text) để trích xuất chữ ra nối câu
    const userSentence = this.arrangeAssembledWords[question.id]
      .map(w => w.text)
      .join(' ')
      .trim();

    // So sánh chuỗi câu của học viên xếp với chuỗi đáp án chuẩn. Nếu khớp 100% thì gán true (Đúng), ngược lại gán false (Sai)
    this.arrangeIsCorrect[question.id] = (userSentence === correctSentence);
  }

  /**
   * Xử lý khi học viên bấm nút "Làm lại" riêng câu hỏi sắp xếp câu đó
   * @param questionId ID câu hỏi sắp xếp cần làm lại
   */
  public resetArrangeQuestion(questionId: number): void {
    this.arrangeAssembledWords[questionId] = []; // Xóa trắng hàng chữ kết quả đã xếp
    this.arrangePoolStatus[questionId] = {};     // Reset trạng thái kho chữ gợi ý về trạng thái chưa ai bấm từ nào
    this.arrangeChecked[questionId] = false;     // Đặt trạng thái câu hỏi về chưa nộp bài/chưa bấm kiểm tra
    this.arrangeIsCorrect[questionId] = false;   // Reset kết quả đúng sai về mặc định ban đầu
  }

  /**
   * Hàm bổ trợ: Trích xuất và ghép các mảnh chữ đáp án riêng lẻ lại thành câu văn đáp án hoàn chỉnh theo đúng thứ tự hiển thị
   * @param question Đối tượng câu hỏi chứa danh sách các đáp án mảnh
   */
  public getCorrectSentenceText(question: QuestionModel): string {
    return question.answers
      .slice() // Tạo bản sao mảng đáp án để tránh làm xáo trộn mảng gốc của câu hỏi

      // Sắp xếp các mảnh đáp án tăng dần dựa theo thuộc tính chuỗi thứ tự displayOrder (Ví dụ: "1", "2", "3"...)
      .sort((a, b) => a.displayOrder.localeCompare(b.displayOrder))

      .map(a => a.text) // Biến đổi mảng đáp án thành một mảng mới chỉ chứa thuộc tính phần chữ (.text) của đáp án
      .join(' ') // Ghép toàn bộ mảng chữ đó lại với nhau thành một chuỗi duy nhất, các chữ cách nhau bằng khoảng trắng
      .trim(); // Cắt bỏ mọi khoảng trắng dư thừa ở đầu câu và cuối câu
  }

  /**
   * Hàm bổ trợ dạng bài điền từ: Thay thế khoảng trống ký hiệu trong đề bài bằng từ cụ thể được chọn để hiển thị ra màn hình
   * @param content Nội dung văn bản gốc chứa khoảng trống (Ví dụ: "日本語が[ ]話せます。")
   * @param text Từ ngữ cần điền vào vị trí trống (Ví dụ: " một chút / すこし")
   */
  public getFilledSentence(content: string, text: string | undefined): string {
    if (!text) return content; // Nếu không truyền chữ nào vào thì giữ nguyên văn bản gốc của đề bài

    // Sử dụng biểu thức chính quy (Regex) để tìm tất cả các ký tự rỗng dạng [] hoặc gạch dưới ___ và thay thế chúng bằng biến text
    return content.replace(/\[\s*\]/g, text).replace(/_+/g, text);
  }

  /**
   * Tính năng phát âm (Text-to-Speech) câu tiếng Nhật bằng công cụ có sẵn của trình duyệt web
   * @param text Đoạn văn bản tiếng Nhật cần phát ra âm thanh đọc
   */
  public speakJapanese(text: string): void {
    if ('speechSynthesis' in window) { // Kiểm tra xem trình duyệt hiện tại của người dùng có hỗ trợ thư viện giọng đọc không

      // Sử dụng Regex loại bỏ các ký tự trống đặc biệt như `[]` hay gạch dưới `_` có trong câu đề bài để giọng đọc không đọc lỗi các ký tự đó
      const cleanText = text.replace(/\[\s*\]/g, '').replace(/_+/g, '');

      const utterance = new SpeechSynthesisUtterance(cleanText); // Tạo một yêu cầu phát âm thanh mới với nội dung chữ đã làm sạch
      utterance.lang = 'ja-JP'; // Cấu hình ngôn ngữ đọc là tiếng Nhật Bản
      utterance.rate = 0.85;    // Cài đặt tốc độ đọc là 0.85 (Đọc chậm hơn bình thường một chút để học viên nghe rõ phát âm ngữ điệu)
      window.speechSynthesis.speak(utterance); // Ra lệnh cho trình duyệt bắt đầu phát giọng đọc câu tiếng Nhật ra loa
    }
  }
}