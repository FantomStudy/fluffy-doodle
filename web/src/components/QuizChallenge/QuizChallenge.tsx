import type { Task } from "@/api/courses";
import { BookOpen, CheckCheck, ChevronRight, RotateCcw } from "lucide-react";
import { useEffect, useState } from "react";
import s from "./QuizChallenge.module.css";

// ─── Types ────────────────────────────────────────────────────────────────────

type QuestionType = "single" | "multiple";

interface Question {
  id: number;
  type: QuestionType;
  text: string;
  code?: string;
  options: string[];
  correct: number[];
  explanation: string;
}

// ─── Theory slides ────────────────────────────────────────────────────────────

const THEORY_SLIDES = [
  {
    icon: "📦",
    title: "Переменная — это ящичек",
    text: "Переменная хранит данные: число, текст, список. Ты даёшь ей имя и кладёшь туда значение. Потом его можно изменить — если объявил через let. Через const — нельзя.",
    codeLabel: "Три способа объявить переменную",
    code: `let age = 12;\nconst name = "Алёша";\nvar score = 100; // устаревший способ`,
  },
  {
    icon: "🔀",
    title: "Условие if / else",
    text: "Условие проверяет — правда или нет. Если правда — выполняется первый блок. Иначе — else. Это как развилка дороги: компьютер идёт по одному из путей.",
    codeLabel: "Пример: проверка оценки",
    code: `let score = 80;\nif (score >= 60) {\n  console.log("Сдал!");\n} else {\n  console.log("Попробуй снова");\n}`,
  },
  {
    icon: "🔁",
    title: "Цикл — это повторение",
    text: "Цикл выполняет блок кода много раз. Цикл for удобен, когда знаешь число повторений. Переменная i считает шаги — от начала до конца.",
    codeLabel: "Считаем от 1 до 3",
    code: `for (let i = 1; i <= 3; i++) {\n  console.log("Шаг " + i);\n}\n// Шаг 1, Шаг 2, Шаг 3`,
  },
  {
    icon: "🎯",
    title: "Готов к тесту?",
    text: "8 вопросов. Где-то нужно выбрать один правильный ответ, где-то — несколько. Читай внимательно — подсказка есть над каждым вопросом. Удачи!",
  },
];

// ─── Questions ────────────────────────────────────────────────────────────────

const QUESTIONS: Question[] = [
  {
    id: 1,
    type: "single",
    text: "Что такое переменная?",
    options: [
      "Функция, которая что-то вычисляет",
      "Ящичек для хранения данных",
      "Команда, которая рисует на экране",
      "Способ остановить программу",
    ],
    correct: [1],
    explanation:
      "Переменная — это именованное место в памяти для хранения значения: числа, текста, списка и т. д.",
  },
  {
    id: 2,
    type: "single",
    text: "Чему равно выражение?",
    code: "2 + 3 * 4",
    options: ["20", "14", "9", "24"],
    correct: [1],
    explanation: "Умножение выполняется раньше сложения: 3 × 4 = 12, затем 2 + 12 = 14.",
  },
  {
    id: 3,
    type: "multiple",
    text: "Выбери все ключевые слова для объявления переменных в JavaScript:",
    options: ["let", "const", "var", "function", "if"],
    correct: [0, 1, 2],
    explanation: "let, const и var объявляют переменные. function — функцию, if — условие.",
  },
  {
    id: 4,
    type: "single",
    text: "Что выведет этот код?",
    code: `let x = 5;\nif (x > 10) {\n  console.log("Большое");\n} else {\n  console.log("Маленькое");\n}`,
    options: ['"Большое"', '"Маленькое"', "Ничего", "Ошибку"],
    correct: [1],
    explanation: '5 > 10 — ложь, поэтому выполняется блок else и выводится "Маленькое".',
  },
  {
    id: 5,
    type: "multiple",
    text: "Какие из этих значений являются «ложными» (falsy) в JavaScript?",
    options: ["0", '""', "null", "1", '"false"'],
    correct: [0, 1, 2],
    explanation:
      'Falsy-значения: 0, "" (пустая строка), null, undefined, false, NaN. Строка "false" — непустая, она истинна. 1 тоже истинна.',
  },
  {
    id: 6,
    type: "single",
    text: "Сколько раз выполнится тело цикла?",
    code: `for (let i = 0; i < 4; i++) {\n  console.log(i);\n}`,
    options: ["3 раза", "4 раза", "5 раз", "0 раз"],
    correct: [1],
    explanation:
      "i принимает значения 0, 1, 2, 3 — 4 итерации. Когда i === 4, условие i < 4 ложно, цикл завершается.",
  },
  {
    id: 7,
    type: "multiple",
    text: "Что верно про переменную, объявленную через const?",
    options: [
      "Её нельзя переназначить",
      "Её нужно сразу инициализировать значением",
      "Её можно изменить позже",
      "Содержимое массива внутри const можно менять",
    ],
    correct: [0, 1, 3],
    explanation:
      "const нельзя переназначить и нужно сразу присвоить значение. Но содержимое объектов и массивов менять можно — сама ссылка не меняется.",
  },
  {
    id: 8,
    type: "single",
    text: "Что делает return внутри функции?",
    options: [
      "Перезапускает функцию",
      "Возвращает значение из функции",
      "Удаляет функцию",
      "Выводит значение в консоль",
    ],
    correct: [1],
    explanation:
      "return отправляет значение обратно туда, откуда функция была вызвана, и немедленно завершает её работу.",
  },
];

// ─── Option state helper ──────────────────────────────────────────────────────

type OptionState = "idle" | "selected" | "correct" | "wrong" | "missed";

function getOptionState(
  i: number,
  selected: Set<number>,
  correct: number[],
  checked: boolean,
): OptionState {
  if (!checked) return selected.has(i) ? "selected" : "idle";
  const isCorrect = correct.includes(i);
  const isSelected = selected.has(i);
  if (isCorrect && isSelected) return "correct";
  if (isCorrect && !isSelected) return "missed";
  if (!isCorrect && isSelected) return "wrong";
  return "idle";
}

// ─── Theory Phase ─────────────────────────────────────────────────────────────

function TheoryPhase({ onFinish }: { onFinish: () => void }) {
  const [idx, setIdx] = useState(0);
  const slide = THEORY_SLIDES[idx];
  const isLast = idx === THEORY_SLIDES.length - 1;

  return (
    <div className={s.theoryShell}>
      <div className={s.theoryProgress}>
        <div
          className={s.theoryProgressFill}
          style={{ width: `${((idx + 1) / THEORY_SLIDES.length) * 100}%` }}
        />
      </div>

      <div className={s.theoryBody}>
        <div className={s.theoryCard} key={idx}>
          <div className={s.theoryCardIcon}>{slide.icon}</div>
          <p className={s.theoryCardStep}>
            Слайд {idx + 1} из {THEORY_SLIDES.length}
          </p>
          <h2 className={s.theoryCardTitle}>{slide.title}</h2>
          <p className={s.theoryCardText}>{slide.text}</p>
          {"code" in slide && slide.code && (
            <div className={s.theoryCodeBlock}>
              {"codeLabel" in slide && slide.codeLabel && (
                <div className={s.theoryCodeLabel}>{slide.codeLabel}</div>
              )}
              <pre className={s.theoryCode}>{slide.code}</pre>
            </div>
          )}
        </div>
      </div>

      <div className={s.theoryNav}>
        <button
          type="button"
          className={`${s.btnNav} ${s.btnNavBack}`}
          onClick={() => setIdx((i) => Math.max(0, i - 1))}
          style={{ visibility: idx === 0 ? "hidden" : "visible" }}
        >
          Назад
        </button>

        <div className={s.theoryDots}>
          {THEORY_SLIDES.map((_, i) => (
            <div key={i} className={`${s.theoryDot} ${i === idx ? s.theoryDotActive : ""}`} />
          ))}
        </div>

        {isLast ? (
          <button type="button" className={`${s.btnNav} ${s.btnNavStart}`} onClick={onFinish}>
            Начать тест 📝
          </button>
        ) : (
          <button
            type="button"
            className={`${s.btnNav} ${s.btnNavNext}`}
            onClick={() => setIdx((i) => i + 1)}
          >
            Далее
          </button>
        )}
      </div>
    </div>
  );
}

// ─── Results Screen ───────────────────────────────────────────────────────────

function ResultsScreen({
  correct,
  total,
  onRetry,
  onTheory,
}: {
  correct: number;
  total: number;
  onRetry: () => void;
  onTheory: () => void;
}) {
  const pct = Math.round((correct / total) * 100);
  const xp = Math.round(pct / 10) * 5;

  const emoji = pct === 100 ? "🏆" : pct >= 75 ? "🎉" : pct >= 50 ? "👍" : "💪";
  const msg =
    pct === 100
      ? "Идеальный результат! Ты настоящий программист!"
      : pct >= 75
        ? "Отличная работа! Ещё чуть-чуть — и будет идеал."
        : pct >= 50
          ? "Неплохо! Повтори теорию и попробуй снова."
          : "Не сдавайся — перечитай теорию и ты точно справишься!";

  return (
    <div className={s.resultsShell}>
      <div className={s.resultsCard}>
        <div className={s.resultsEmoji}>{emoji}</div>
        <h1 className={s.resultsTitle}>Тест завершён!</h1>

        <div className={s.resultsScoreRow}>
          <span className={s.scoreNum}>{correct}</span>
          <span className={s.scoreSep}>/</span>
          <span className={s.scoreTotal}>{total}</span>
        </div>

        <div className={s.resultBar}>
          <div
            className={`${s.resultBarFill} ${pct >= 75 ? s.resultBarGreen : pct >= 50 ? s.resultBarYellow : s.resultBarRed}`}
            style={{ width: `${pct}%` }}
          />
        </div>
        <div className={s.resultPct}>{pct}% правильных ответов</div>

        <p className={s.resultsMsg}>{msg}</p>
        <div className={s.xpBadge}>⭐ +{xp} XP</div>

        <div className={s.resultsBtns}>
          <button type="button" className={s.btnTheory} onClick={onTheory}>
            <BookOpen size={14} /> Повторить теорию
          </button>
          <button type="button" className={s.btnRetry} onClick={onRetry}>
            <RotateCcw size={14} /> Пройти снова
          </button>
        </div>
      </div>
    </div>
  );
}

// ─── Quiz Phase ───────────────────────────────────────────────────────────────

function QuizPhase({
  onBackToTheory,
  onComplete,
}: {
  onBackToTheory: () => void;
  onComplete?: () => void;
}) {
  const [qIdx, setQIdx] = useState(0);
  const [selected, setSelected] = useState<Set<number>>(() => new Set());
  const [checked, setChecked] = useState(false);
  const [correctCount, setCorrectCount] = useState(0);
  const [finished, setFinished] = useState(false);

  const q = QUESTIONS[qIdx];
  const total = QUESTIONS.length;

  const isAnswerCorrect =
    selected.size === q.correct.length && q.correct.every((i) => selected.has(i));

  const toggle = (i: number) => {
    if (checked) return;
    if (q.type === "single") {
      setSelected(new Set([i]));
    } else {
      setSelected((prev) => {
        const next = new Set(prev);
        if (next.has(i)) next.delete(i);
        else next.add(i);
        return next;
      });
    }
  };

  const check = () => {
    if (selected.size === 0) return;
    if (isAnswerCorrect) setCorrectCount((c) => c + 1);
    setChecked(true);
  };

  const next = () => {
    if (qIdx === total - 1) {
      setFinished(true);
      onComplete?.();
    } else {
      setQIdx((i) => i + 1);
      setSelected(new Set());
      setChecked(false);
    }
  };

  const retry = () => {
    setQIdx(0);
    setSelected(new Set());
    setChecked(false);
    setCorrectCount(0);
    setFinished(false);
  };

  if (finished) {
    return (
      <ResultsScreen
        correct={correctCount}
        total={total}
        onRetry={retry}
        onTheory={onBackToTheory}
      />
    );
  }

  return (
    <div className={s.quizShell}>
      {/* Header */}
      <div className={s.quizHeader}>
        <span className={s.qCounter}>
          Вопрос {qIdx + 1} из {total}
        </span>
        <div className={s.qProgressBar}>
          <div className={s.qProgressFill} style={{ width: `${((qIdx + 1) / total) * 100}%` }} />
        </div>
        <span className={s.qScore}>✅ {correctCount}</span>
      </div>

      {/* Card */}
      <div className={s.quizBody}>
        <div className={s.qCard} key={qIdx}>
          <div className={`${s.qTypeBadge} ${q.type === "multiple" ? s.qTypeBadgeMulti : ""}`}>
            {q.type === "single" ? "Один правильный ответ" : "Несколько правильных ответов"}
          </div>

          <h2 className={s.qText}>{q.text}</h2>

          {q.code && <pre className={s.qCode}>{q.code}</pre>}

          <div className={s.optionsList}>
            {q.options.map((opt, i) => {
              const state = getOptionState(i, selected, q.correct, checked);
              return (
                <button
                  key={i}
                  type="button"
                  className={`${s.option} ${s[`option_${state}`]}`}
                  onClick={() => toggle(i)}
                  disabled={checked && state === "idle"}
                >
                  <span
                    className={`${s.optionMarker} ${q.type === "multiple" ? s.optionMarkerCheck : s.optionMarkerRadio}`}
                  >
                    {state === "correct" && "✓"}
                    {state === "wrong" && "✗"}
                    {state === "missed" && "○"}
                  </span>
                  <span className={s.optionText}>{opt}</span>
                </button>
              );
            })}
          </div>

          {checked && (
            <div
              className={`${s.explanation} ${isAnswerCorrect ? s.explanationGood : s.explanationBad}`}
            >
              <span className={s.explanationBadge}>
                {isAnswerCorrect ? "✅ Верно!" : "❌ Не совсем..."}
              </span>
              {q.explanation}
            </div>
          )}
        </div>
      </div>

      {/* Footer */}
      <div className={s.quizFooter}>
        {!checked ? (
          <button
            type="button"
            className={s.btnCheck}
            onClick={check}
            disabled={selected.size === 0}
          >
            <CheckCheck size={15} /> Проверить
          </button>
        ) : (
          <button type="button" className={s.btnNext} onClick={next}>
            {qIdx === total - 1 ? "Посмотреть результат 🏆" : "Следующий вопрос"}
            <ChevronRight size={15} />
          </button>
        )}
      </div>
    </div>
  );
}

// ─── API Task Quiz ────────────────────────────────────────────────────────────

function ApiQuizPhase({
  task,
  onComplete,
}: {
  task: Task;
  onComplete?: (selectedOptionIds: string[]) => void;
}) {
  const [selected, setSelected] = useState<Set<string>>(new Set());
  const [submitted, setSubmitted] = useState(false);

  const toggle = (optionId: string) => {
    if (submitted) return;
    setSelected((prev) => {
      const next = new Set(prev);
      if (next.has(optionId)) next.delete(optionId);
      else next.add(optionId);
      return next;
    });
  };

  const handleSubmit = () => {
    const ids = Array.from(selected);
    setSubmitted(true);
    onComplete?.(ids);
  };

  return (
    <div className={s.quizShell}>
      <div className={s.quizHeader}>
        <span className={s.qCounter}>{task.title}</span>
        <div className={s.qProgressBar}>
          <div className={s.qProgressFill} style={{ width: "100%" }} />
        </div>
        <span className={s.qScore}>
          ⭐ {task.rewardStars} | +{task.rewardExp} XP
        </span>
      </div>

      <div className={s.quizBody}>
        <div className={s.qCard}>
          <div className={s.qTypeBadge}>Выбери правильные ответы</div>
          <h2 className={s.qText}>{task.question}</h2>

          <div className={s.optionsList}>
            {task.options?.map((opt) => {
              const isSelected = selected.has(opt.id);
              const stateClass = submitted
                ? isSelected
                  ? s.option_selected
                  : s.option_idle
                : isSelected
                  ? s.option_selected
                  : s.option_idle;
              return (
                <button
                  key={opt.id}
                  type="button"
                  className={`${s.option} ${stateClass}`}
                  onClick={() => toggle(opt.id)}
                  disabled={submitted}
                >
                  <span className={`${s.optionMarker} ${s.optionMarkerCheck}`}>
                    {isSelected && "✓"}
                  </span>
                  <span className={s.optionText}>{opt.text}</span>
                </button>
              );
            })}
          </div>

          {submitted && (
            <div className={`${s.explanation} ${s.explanationGood}`}>
              <span className={s.explanationBadge}>✅ Ответ отправлен!</span>
              Твой результат обрабатывается
            </div>
          )}
        </div>
      </div>

      <div className={s.quizFooter}>
        {!submitted && (
          <button
            type="button"
            className={s.btnCheck}
            onClick={handleSubmit}
            disabled={selected.size === 0}
          >
            <CheckCheck size={15} /> Отправить ответ
          </button>
        )}
      </div>
    </div>
  );
}

// ─── Root ─────────────────────────────────────────────────────────────────────

interface QuizChallengeProps {
  task?: Task;
  onComplete?: (selectedOptionIds?: string[]) => void;
}

export function QuizChallenge({ task, onComplete }: QuizChallengeProps = {}) {
  const [phase, setPhase] = useState<"theory" | "quiz">("theory");

  useEffect(() => {
    const prevClass = document.body.className;
    document.body.className = "";
    document.body.style.background = "#f5f5f7";
    return () => {
      document.body.className = prevClass;
      document.body.style.background = "";
    };
  }, []);

  if (task) {
    return (
      <main className={s.main}>
        {phase === "theory" ? (
          <TheoryPhase onFinish={() => setPhase("quiz")} />
        ) : (
          <ApiQuizPhase task={task} onComplete={onComplete} />
        )}
      </main>
    );
  }

  return (
    <main className={s.main}>
      {phase === "theory" ? (
        <TheoryPhase onFinish={() => setPhase("quiz")} />
      ) : (
        <QuizPhase onBackToTheory={() => setPhase("theory")} onComplete={onComplete} />
      )}
    </main>
  );
}
