import Editor from "@monaco-editor/react";
import {
  BookOpen,
  ChevronLeft,
  ChevronRight,
  ClipboardList,
  FileText,
  FlaskConical,
  Home,
  Map,
  Play,
  RotateCcw,
  Trophy,
  User,
  Users,
} from "lucide-react";
import { useCallback, useEffect, useState } from "react";
import s from "./CodingChallenge.module.css";

// ─── Types ────────────────────────────────────────────────────────────────────

export interface TestCase {
  id: number;
  input: string;
  expected: string;
  description: string;
}

export interface Solution {
  author: string;
  date: string;
  code: string;
  likes: number;
}

export interface TheorySlide {
  icon: string;
  title: string;
  text: string;
  code?: { label: string; line: string; result: string };
}

export interface ChallengeData {
  id: string;
  title: string;
  taskName: string;
  language: "javascript" | "typescript" | "python";
  languageLabel: string;
  difficulty: 1 | 2 | 3 | 4 | 5;
  xp: number;
  theorySlides: TheorySlide[];
  description: string;
  examples: { call: string; result: string }[];
  starterCode: string;
  testCases: TestCase[];
  solutions: Solution[];
}

// ─── Default: very simple first task ─────────────────────────────────────────

export const DEFAULT_CHALLENGE: ChallengeData = {
  id: "double",
  title: "Удвой число",
  taskName: "double",
  language: "javascript",
  languageLabel: "JavaScript",
  difficulty: 1,
  xp: 5,
  theorySlides: [
    {
      icon: "🧮",
      title: "Что такое функция?",
      text: "Функция — это маленькая машинка. Ты даёшь ей число на входе, она что-то делает и отдаёт результат.",
      code: {
        label: "Так выглядит функция",
        line: "function hello() {\n  return \"Привет!\";\n}",
        result: "→ \"Привет!\"",
      },
    },
    {
      icon: "📥",
      title: "Параметр — это вход",
      text: "Чтобы передать число в функцию, используй скобки. Внутри скобок — имя для числа, которое ты передаёшь.",
      code: {
        label: "Число передаётся через параметр",
        line: "function double(n) {\n  // n — это переданное число\n}",
        result: "double(5)  →  n = 5",
      },
    },
    {
      icon: "✖️",
      title: "Умножение и return",
      text: "Чтобы посчитать и отдать результат обратно, напиши return. После него — то, что функция вернёт.",
      code: {
        label: "Умножь n на 2 и верни",
        line: "function double(n) {\n  return n * 2;\n}",
        result: "double(5)  →  10",
      },
    },
    {
      icon: "🎯",
      title: "Попробуй сам!",
      text: "Теперь твоя очередь. Напиши функцию double(n), которая принимает число и возвращает его, умноженное на 2. Это совсем просто!",
      code: {
        label: "Что должно получиться",
        line: "double(3)   →  6\ndouble(10)  →  20\ndouble(0)   →  0",
        result: "",
      },
    },
  ],
  description:
    "Напиши функцию double(n). Она принимает одно число n и возвращает это число, умноженное на 2.",
  examples: [
    { call: "double(3)", result: "// 6" },
    { call: "double(10)", result: "// 20" },
    { call: "double(0)", result: "// 0" },
  ],
  starterCode: `function double(n) {\n  // Напиши код здесь\n}`,
  testCases: [
    { id: 1, input: "double(3)", expected: "6", description: "double(3) должно быть 6" },
    { id: 2, input: "double(10)", expected: "20", description: "double(10) должно быть 20" },
    { id: 3, input: "double(0)", expected: "0", description: "double(0) должно быть 0" },
    { id: 4, input: "double(7)", expected: "14", description: "double(7) должно быть 14" },
  ],
  solutions: [
    {
      author: "nastya_kd",
      date: "вчера",
      code: `function double(n) {\n  return n * 2;\n}`,
      likes: 31,
    },
    {
      author: "ilya_prog",
      date: "3 дня назад",
      code: `function double(n) {\n  return n + n;\n}`,
      likes: 18,
    },
  ],
};

// ─── Helpers ──────────────────────────────────────────────────────────────────

interface TestResult {
  id: number;
  description: string;
  input: string;
  expected: string;
  got: string;
  passed: boolean;
}

function runTests(code: string, testCases: TestCase[]): TestResult[] {
  return testCases.map((tc) => {
    let got = "";
    let passed = false;
    try {
      // eslint-disable-next-line no-new-func
      const fn = new Function(`${code}; return ${tc.input};`);
      got = String(fn());
      passed = got === tc.expected;
    }
    catch (err) {
      got = err instanceof Error ? err.message : String(err);
    }
    return { id: tc.id, description: tc.description, input: tc.input, expected: tc.expected, got, passed };
  });
}

function Stars({ count, total = 5 }: { count: number; total?: number }) {
  return (
    <span className={s.stars}>
      {Array.from({ length: total }, (_, i) => (
        <span key={i} className={i < count ? s.starOn : s.starOff}>★</span>
      ))}
    </span>
  );
}

// ─── Sidebar ──────────────────────────────────────────────────────────────────

const NAV = [
  { icon: <Home size={16} />, label: "Главная" },
  { icon: <Map size={16} />, label: "Путь" },
  { icon: <BookOpen size={16} />, label: "Уроки" },
  { icon: <ClipboardList size={16} />, label: "Задания", active: true },
  { icon: <Trophy size={16} />, label: "Достижения" },
  { icon: <User size={16} />, label: "Профиль" },
];

function Sidebar() {
  return (
    <aside className={s.sidebar}>
      <div className={s.sidebarTop}>
        <div className={s.avatar} />
        <span className={s.brand}>Orbita Kids</span>
      </div>
      <nav className={s.nav}>
        {NAV.map(item => (
          <button
            key={item.label}
            type="button"
            className={`${s.navItem} ${item.active ? s.navItemActive : ""}`}
          >
            {item.icon}
            {item.label}
          </button>
        ))}
      </nav>
    </aside>
  );
}

// ─── Theory phase ─────────────────────────────────────────────────────────────

function TheoryPhase({
  slides,
  onFinish,
}: {
  slides: TheorySlide[];
  onFinish: () => void;
}) {
  const [idx, setIdx] = useState(0);
  const slide = slides[idx];
  const isLast = idx === slides.length - 1;

  return (
    <div className={s.theoryShell}>
      {/* Progress bar */}
      <div className={s.theoryProgress}>
        <div
          className={s.theoryProgressFill}
          style={{ width: `${((idx + 1) / slides.length) * 100}%` }}
        />
      </div>

      {/* Slide */}
      <div className={s.theoryBody}>
        <div className={s.theoryCard} key={idx}>
          <div className={s.theoryCardIcon}>{slide.icon}</div>
          <p className={s.theoryCardStep}>Шаг {idx + 1} из {slides.length}</p>
          <h2 className={s.theoryCardTitle}>{slide.title}</h2>
          <p className={s.theoryCardText}>{slide.text}</p>
          {slide.code && (
            <div className={s.theoryCode}>
              <span className={s.theoryCodeLabel}>{slide.code.label}</span>
              <pre className={s.theoryCodeLine}>{slide.code.line}</pre>
              {slide.code.result && (
                <pre className={s.theoryCodeResult}>{slide.code.result}</pre>
              )}
            </div>
          )}
        </div>
      </div>

      {/* Nav */}
      <div className={s.theoryNav}>
        <button
          type="button"
          className={`${s.btnNav} ${s.btnNavBack}`}
          onClick={() => setIdx(i => Math.max(0, i - 1))}
          disabled={idx === 0}
          style={{ visibility: idx === 0 ? "hidden" : "visible" }}
        >
          <ChevronLeft size={15} />
          Назад
        </button>

        <div className={s.theoryDots}>
          {slides.map((_, i) => (
            <div
              key={i}
              className={`${s.theoryDot} ${i === idx ? s.theoryDotActive : ""}`}
            />
          ))}
        </div>

        {isLast
          ? (
              <button type="button" className={`${s.btnNav} ${s.btnNavStart}`} onClick={onFinish}>
                Начать задание 🚀
              </button>
            )
          : (
              <button
                type="button"
                className={`${s.btnNav} ${s.btnNavNext}`}
                onClick={() => setIdx(i => i + 1)}
              >
                Далее
                <ChevronRight size={15} />
              </button>
            )}
      </div>
    </div>
  );
}

// ─── Task phase ───────────────────────────────────────────────────────────────

type TabId = "description" | "theory" | "solutions";

function TaskPhase({ challenge }: { challenge: ChallengeData }) {
  const [tab, setTab] = useState<TabId>("description");
  const [code, setCode] = useState(challenge.starterCode);
  const [results, setResults] = useState<TestResult[] | null>(null);
  const [running, setRunning] = useState(false);

  const passed = results?.filter(r => r.passed).length ?? 0;
  const total = challenge.testCases.length;
  const allPassed = results !== null && passed === total;

  const handleRun = useCallback(() => {
    if (running) return;
    setRunning(true);
    setTimeout(() => {
      setResults(runTests(code, challenge.testCases));
      setRunning(false);
    }, 250);
  }, [code, challenge.testCases, running]);

  const handleReset = useCallback(() => {
    setCode(challenge.starterCode);
    setResults(null);
  }, [challenge.starterCode]);

  const tabs: { id: TabId; icon: React.ReactNode; label: string }[] = [
    { id: "description", icon: <FileText size={13} />, label: "Задание" },
    { id: "theory", icon: <BookOpen size={13} />, label: "Подсказка" },
    { id: "solutions", icon: <Users size={13} />, label: "Решения" },
  ];

  return (
    <div className={s.taskShell}>
      {/* Left */}
      <div className={s.leftPanel}>
        <div className={s.leftHeader}>
          <h1 className={s.leftTitle}>{challenge.title}</h1>
          <div className={s.leftMeta}>
            <span className={`${s.pill} ${s.pillPurple}`}>🟨 {challenge.languageLabel}</span>
            <Stars count={challenge.difficulty} />
            <span className={`${s.pill} ${s.pillGreen}`}>⭐ {challenge.xp} XP</span>
          </div>
        </div>

        <div className={s.tabBar}>
          {tabs.map(t => (
            <button
              key={t.id}
              type="button"
              className={`${s.tab} ${tab === t.id ? s.tabActive : ""}`}
              onClick={() => setTab(t.id)}
            >
              {t.icon}
              {t.label}
            </button>
          ))}
        </div>

        <div className={s.tabContent}>
          {tab === "description" && (
            <>
              <p className={s.descText}>{challenge.description}</p>
              <p className={s.sectionLabel}>Примеры</p>
              {challenge.examples.map((ex, i) => (
                <div key={i} className={s.exampleBlock}>
                  <span>{ex.call}</span>
                  {"  "}
                  <span className={s.exampleComment}>{ex.result}</span>
                </div>
              ))}
              <div className={s.hintPill}>🔒 Подсказка за 5 XP</div>
            </>
          )}

          {tab === "theory" && (
            <>
              <p className={s.sectionLabel} style={{ marginBottom: "12px" }}>Что мы узнали</p>
              <div className={s.reviewList}>
                {challenge.theorySlides.slice(0, -1).map((sl, i) => (
                  <div key={i} className={s.reviewItem}>
                    <div className={s.reviewNum}>{i + 1}</div>
                    <div>
                      <p className={s.reviewItemTitle}>{sl.title}</p>
                      <p className={s.reviewItemText}>{sl.text}</p>
                    </div>
                  </div>
                ))}
              </div>
              {challenge.theorySlides[2]?.code && (
                <div className={s.reviewCode}>
                  <span>{challenge.theorySlides[2].code.line}</span>
                  <span className={s.reviewCodeResult}>{challenge.theorySlides[2].code.result}</span>
                </div>
              )}
            </>
          )}

          {tab === "solutions" && (
            <>
              {challenge.solutions.map((sol, i) => (
                <div key={i} className={s.solutionCard}>
                  <div className={s.solutionMeta}>
                    <span className={s.solutionAuthor}>{sol.author}</span>
                    <span>{sol.date}</span>
                    <span style={{ marginLeft: "auto" }}>👍 {sol.likes}</span>
                  </div>
                  <pre className={s.solutionCode}>{sol.code}</pre>
                </div>
              ))}
            </>
          )}
        </div>
      </div>

      {/* Right */}
      <div className={s.rightPanel}>
        <div className={s.editorBar}>
          <span className={s.editorBarTitle}>{challenge.taskName}.js</span>
          <div className={s.editorBarActions}>
            <button type="button" className={s.btnReset} onClick={handleReset}>
              <RotateCcw size={13} />
              Сбросить
            </button>
            <button type="button" className={s.btnRun} onClick={handleRun} disabled={running}>
              <Play size={13} />
              {running ? "Проверяю…" : "Запустить"}
            </button>
          </div>
        </div>

        <div className={s.editorWrap}>
          <Editor
            height="100%"
            language={challenge.language}
            value={code}
            onChange={v => setCode(v ?? "")}
            theme="vs-dark"
            options={{
              fontSize: 14,
              fontFamily: "'Fira Code','Cascadia Code','Consolas',monospace",
              minimap: { enabled: false },
              scrollBeyondLastLine: false,
              lineNumbers: "on",
              tabSize: 2,
              padding: { top: 16, bottom: 16 },
              scrollbar: { verticalScrollbarSize: 5 },
              renderLineHighlight: "line",
            }}
          />
        </div>

        <div className={s.resultsPanel}>
          <div className={s.resultsPanelBar}>
            <FlaskConical size={13} color="#565f89" />
            <span className={s.resultsPanelTitle}>Тесты</span>
            {results && (
              <span className={s.resultsPanelScore}>{passed}/{total} пройдено</span>
            )}
          </div>

          {allPassed && (
            <div className={s.successBanner}>🎉 Все тесты пройдены! +{challenge.xp} XP</div>
          )}

          <div className={s.resultsPanelBody}>
            {!results
              ? <span className={s.resultEmpty}>Нажми «Запустить» чтобы проверить…</span>
              : results.map(r => (
                  <div
                    key={r.id}
                    className={`${s.resultItem} ${r.passed ? s.resultItemPass : s.resultItemFail}`}
                  >
                    <span className={s.resultIcon}>{r.passed ? "✅" : "❌"}</span>
                    <div>
                      <div className={s.resultDesc}>{r.description}</div>
                      <div className={s.resultCode}>{r.input}</div>
                      {!r.passed && (
                        <>
                          <div className={s.resultExpected}>Ожидалось: {r.expected}</div>
                          <div className={s.resultGot}>Получено: {r.got}</div>
                        </>
                      )}
                    </div>
                  </div>
                ))}
          </div>
        </div>
      </div>
    </div>
  );
}

// ─── Root ─────────────────────────────────────────────────────────────────────

interface CodingChallengeProps {
  challenge?: ChallengeData;
}

export function CodingChallenge({ challenge = DEFAULT_CHALLENGE }: CodingChallengeProps) {
  const [phase, setPhase] = useState<"theory" | "task">("theory");

  useEffect(() => {
    const prev = document.body.className;
    const prevBg = document.body.style.background;
    document.body.className = "";
    document.body.style.background = "#f5f5f7";
    return () => {
      document.body.className = prev;
      document.body.style.background = prevBg;
    };
  }, []);

  return (
    <div className={s.page}>
      <Sidebar />
      {phase === "theory"
        ? <TheoryPhase slides={challenge.theorySlides} onFinish={() => setPhase("task")} />
        : <TaskPhase challenge={challenge} />}
    </div>
  );
}
