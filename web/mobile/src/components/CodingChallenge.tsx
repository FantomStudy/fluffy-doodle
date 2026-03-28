import type { Task } from "../api/courses";
import * as React from "react";
import { useCallback, useState } from "react";
import { Pressable, ScrollView, StyleSheet, Text, TextInput, View } from "react-native";
import { colors, radii, semantic } from "../theme/tokens";

export interface TestCase {
  id: number;
  input: string;
  expected: string;
  description: string;
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
}

const DEFAULT_CHALLENGE: ChallengeData = {
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
      text: "Функция — это маленькая машинка. Ты даёшь ей число, она что-то делает и отдаёт результат.",
      code: {
        label: "Так выглядит функция",
        line: 'function hello() {\n  return "Привет!";\n}',
        result: '→ "Привет!"',
      },
    },
    {
      icon: "📥",
      title: "Параметр — это вход",
      text: "Чтобы передать число в функцию, используй скобки.",
      code: {
        label: "Число через параметр",
        line: "function double(n) {\n  // n — число\n}",
        result: "double(5) → n = 5",
      },
    },
    {
      icon: "✖️",
      title: "Умножение и return",
      text: "Чтобы отдать результат, напиши return.",
      code: {
        label: "Умножь n на 2",
        line: "function double(n) {\n  return n * 2;\n}",
        result: "double(5) → 10",
      },
    },
    {
      icon: "🎯",
      title: "Попробуй сам!",
      text: "Напиши функцию double(n), которая возвращает число, умноженное на 2.",
    },
  ],
  description: "Напиши функцию double(n). Она принимает число n и возвращает его × 2.",
  examples: [
    { call: "double(3)", result: "// 6" },
    { call: "double(10)", result: "// 20" },
  ],
  starterCode: "function double(n) {\n  // Напиши код здесь\n}",
  testCases: [
    { id: 1, input: "double(3)", expected: "6", description: "double(3) → 6" },
    { id: 2, input: "double(10)", expected: "20", description: "double(10) → 20" },
    { id: 3, input: "double(0)", expected: "0", description: "double(0) → 0" },
    { id: 4, input: "double(7)", expected: "14", description: "double(7) → 14" },
  ],
};

interface TestResult {
  id: number;
  description: string;
  expected: string;
  got: string;
  passed: boolean;
}

const TheoryPhase = ({ slides, onFinish }: { slides: TheorySlide[]; onFinish: () => void }) => {
  const [idx, setIdx] = useState(0);
  const slide = slides[idx];
  const isLast = idx === slides.length - 1;

  return (
    <View style={styles.theoryShell}>
      <View style={styles.progressBar}>
        <View
          style={[styles.progressFill, { width: `${((idx + 1) / slides.length) * 100}%` as any }]}
        />
      </View>

      <ScrollView contentContainerStyle={styles.theoryBody}>
        <Text style={styles.theoryIcon}>{slide.icon}</Text>
        <Text style={styles.theoryStep}>
          Шаг {idx + 1} из {slides.length}
        </Text>
        <Text style={styles.theoryTitle}>{slide.title}</Text>
        <Text style={styles.theoryText}>{slide.text}</Text>
        {slide.code && (
          <View style={styles.codeBlock}>
            <Text style={styles.codeLabel}>{slide.code.label}</Text>
            <Text style={styles.codeText}>{slide.code.line}</Text>
            {slide.code.result ? <Text style={styles.codeResult}>{slide.code.result}</Text> : null}
          </View>
        )}
      </ScrollView>

      <View style={styles.navRow}>
        {idx > 0 ? (
          <Pressable style={styles.navBtn} onPress={() => setIdx(idx - 1)}>
            <Text style={styles.navBtnText}>← Назад</Text>
          </Pressable>
        ) : (
          <View />
        )}
        <View style={styles.dots}>
          {slides.map((_, i) => (
            <View key={i} style={[styles.dot, i === idx && styles.dotActive]} />
          ))}
        </View>
        {isLast ? (
          <Pressable style={[styles.navBtn, styles.navBtnStart]} onPress={onFinish}>
            <Text style={styles.navBtnStartText}>Начать 🚀</Text>
          </Pressable>
        ) : (
          <Pressable style={[styles.navBtn, styles.navBtnNext]} onPress={() => setIdx(idx + 1)}>
            <Text style={styles.navBtnNextText}>Далее →</Text>
          </Pressable>
        )}
      </View>
    </View>
  );
};

const TaskPhase = ({
  challenge,
  onComplete,
}: {
  challenge: ChallengeData;
  onComplete?: () => void;
}) => {
  const [code, setCode] = useState(challenge.starterCode);
  const [results, setResults] = useState<TestResult[] | null>(null);
  const [running, setRunning] = useState(false);

  const passed = results?.filter((r) => r.passed).length ?? 0;
  const total = challenge.testCases.length;
  const allPassed = results !== null && passed === total;

  const handleRun = useCallback(() => {
    if (running) return;
    setRunning(true);
    setTimeout(() => {
      const testResults = challenge.testCases.map((tc) => {
        let got = "";
        let testPassed = false;
        try {
          // eslint-disable-next-line no-new-func
          const fn = new Function(`${code}; return ${tc.input};`);
          got = String(fn());
          testPassed = got === tc.expected;
        } catch (err) {
          got = err instanceof Error ? err.message : String(err);
        }
        return {
          id: tc.id,
          description: tc.description,
          expected: tc.expected,
          got,
          passed: testPassed,
        };
      });
      setResults(testResults);
      setRunning(false);
      if (testResults.every((r) => r.passed)) onComplete?.();
    }, 300);
  }, [code, challenge.testCases, running, onComplete]);

  return (
    <ScrollView style={styles.taskShell} contentContainerStyle={styles.taskContent}>
      <Text style={styles.taskTitle}>{challenge.title}</Text>
      <View style={styles.taskMeta}>
        <Text style={styles.pill}>🟨 {challenge.languageLabel}</Text>
        <Text style={styles.pill}>⭐ {challenge.xp} XP</Text>
      </View>

      <Text style={styles.descText}>{challenge.description}</Text>

      {challenge.examples.length > 0 && (
        <View style={styles.examplesWrap}>
          <Text style={styles.sectionLabel}>Примеры</Text>
          {challenge.examples.map((ex, i) => (
            <View key={i} style={styles.exampleBlock}>
              <Text style={styles.codeText}>
                {ex.call} {ex.result}
              </Text>
            </View>
          ))}
        </View>
      )}

      <Text style={styles.sectionLabel}>Код</Text>
      <View style={styles.editorWrap}>
        <TextInput
          style={styles.editor}
          multiline
          value={code}
          onChangeText={setCode}
          autoCapitalize="none"
          autoCorrect={false}
          spellCheck={false}
          textAlignVertical="top"
        />
      </View>

      <View style={styles.editorActions}>
        <Pressable
          style={styles.btnReset}
          onPress={() => {
            setCode(challenge.starterCode);
            setResults(null);
          }}
        >
          <Text style={styles.btnResetText}>🔄 Сбросить</Text>
        </Pressable>
        <Pressable
          style={[styles.btnRun, running && styles.btnDisabled]}
          onPress={handleRun}
          disabled={running}
        >
          <Text style={styles.btnRunText}>{running ? "Проверяю…" : "▶ Запустить"}</Text>
        </Pressable>
      </View>

      <Text style={styles.sectionLabel}>Тесты</Text>
      {allPassed && (
        <Text style={styles.successBanner}>🎉 Все тесты пройдены! +{challenge.xp} XP</Text>
      )}
      {!results ? (
        <Text style={styles.resultEmpty}>Нажми «Запустить» чтобы проверить…</Text>
      ) : (
        results.map((r) => (
          <View
            key={r.id}
            style={[
              styles.resultItem,
              { borderLeftColor: r.passed ? colors.green600 : colors.red400 },
            ]}
          >
            <Text style={styles.resultIcon}>{r.passed ? "✅" : "❌"}</Text>
            <View style={{ flex: 1 }}>
              <Text style={styles.resultDesc}>{r.description}</Text>
              {!r.passed && (
                <>
                  <Text style={styles.resultExpected}>Ожидалось: {r.expected}</Text>
                  <Text style={styles.resultGot}>Получено: {r.got}</Text>
                </>
              )}
            </View>
          </View>
        ))
      )}
    </ScrollView>
  );
};

const buildChallengeFromTask = (task: Task): ChallengeData => {
  const fnName =
    task.title
      .toLowerCase()
      .replace(/\s+/g, "_")
      .replace(/[^a-z0-9_]/g, "") || "solve";
  return {
    id: String(task.id),
    title: task.title,
    taskName: fnName,
    language: "javascript",
    languageLabel: "JavaScript",
    difficulty: 1,
    xp: task.rewardExp,
    theorySlides: [
      {
        icon: "📖",
        title: "Что нужно сделать",
        text: task.description || "Напиши код для решения задачи.",
        code: {
          label: "Подсказка",
          line: `function ${fnName}() {\n  // Твой код\n}`,
          result: "→ напиши решение",
        },
      },
      {
        icon: "🎯",
        title: "Попробуй сам!",
        text: `Напиши функцию ${fnName}(). За это ⭐ ${task.rewardStars} звёзд и +${task.rewardExp} XP!`,
      },
    ],
    description: `${task.question || task.description}\n\nНапиши решение и нажми «Запустить».`,
    examples: [],
    starterCode: `function ${fnName}() {\n  // Напиши код\n\n}`,
    testCases: [],
  };
};

interface CodingChallengeProps {
  task?: Task;
  challenge?: ChallengeData;
  onComplete?: () => void;
}

export const CodingChallenge = ({ task, challenge, onComplete }: CodingChallengeProps) => {
  const resolvedChallenge = challenge ?? (task ? buildChallengeFromTask(task) : DEFAULT_CHALLENGE);
  const [phase, setPhase] = useState<"theory" | "task">("theory");

  return phase === "theory" ? (
    <TheoryPhase slides={resolvedChallenge.theorySlides} onFinish={() => setPhase("task")} />
  ) : (
    <TaskPhase challenge={resolvedChallenge} onComplete={onComplete} />
  );
};

const styles = StyleSheet.create({
  theoryShell: { flex: 1, backgroundColor: semantic.background },
  progressBar: {
    height: 6,
    backgroundColor: semantic.borderSubtle,
    borderRadius: radii.full,
    marginHorizontal: 16,
    marginTop: 12,
  },
  progressFill: { height: "100%", backgroundColor: colors.purple600, borderRadius: radii.full },
  theoryBody: { padding: 24, alignItems: "center", gap: 12 },
  theoryIcon: { fontSize: 48 },
  theoryStep: { fontSize: 13, color: semantic.foregroundMuted },
  theoryTitle: { fontSize: 24, fontWeight: "700", color: semantic.foreground, textAlign: "center" },
  theoryText: {
    fontSize: 16,
    color: semantic.foregroundStrong,
    textAlign: "center",
    lineHeight: 24,
  },
  codeBlock: {
    width: "100%",
    backgroundColor: "#1e2030",
    borderRadius: radii.xl,
    padding: 16,
    gap: 4,
  },
  codeLabel: { fontSize: 11, fontWeight: "600", color: colors.editorTextMuted, marginBottom: 4 },
  codeText: { fontFamily: "monospace", fontSize: 13, color: "#c8d3f5", lineHeight: 20 },
  codeResult: { fontFamily: "monospace", fontSize: 12, color: colors.green400, marginTop: 4 },
  navRow: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
    padding: 16,
    gap: 8,
  },
  navBtn: { paddingHorizontal: 16, paddingVertical: 10 },
  navBtnText: { fontSize: 14, color: semantic.foregroundMuted },
  navBtnNext: {
    backgroundColor: colors.purple600,
    borderRadius: radii.full,
    paddingHorizontal: 20,
  },
  navBtnNextText: { fontSize: 14, fontWeight: "600", color: semantic.foregroundWhite },
  navBtnStart: {
    backgroundColor: colors.purple600,
    borderRadius: radii.full,
    paddingHorizontal: 20,
  },
  navBtnStartText: { fontSize: 14, fontWeight: "600", color: semantic.foregroundWhite },
  dots: { flexDirection: "row", gap: 6 },
  dot: { width: 8, height: 8, borderRadius: 4, backgroundColor: semantic.borderSubtle },
  dotActive: { backgroundColor: colors.purple600 },
  taskShell: { flex: 1, backgroundColor: semantic.background },
  taskContent: { padding: 16, gap: 16, paddingBottom: 100 },
  taskTitle: { fontSize: 24, fontWeight: "700", color: semantic.foreground },
  taskMeta: { flexDirection: "row", gap: 8 },
  pill: {
    fontSize: 12,
    fontWeight: "600",
    color: colors.purple600,
    backgroundColor: colors.purple50,
    paddingHorizontal: 10,
    paddingVertical: 4,
    borderRadius: radii.full,
    overflow: "hidden",
  },
  descText: { fontSize: 15, color: semantic.foregroundStrong, lineHeight: 22 },
  examplesWrap: { gap: 8 },
  sectionLabel: { fontSize: 14, fontWeight: "700", color: semantic.foreground },
  exampleBlock: { backgroundColor: "#1e2030", borderRadius: radii.md, padding: 10 },
  editorWrap: { backgroundColor: "#1e2030", borderRadius: radii.xl, minHeight: 180, padding: 12 },
  editor: { fontFamily: "monospace", fontSize: 14, color: "#c8d3f5", minHeight: 160 },
  editorActions: { flexDirection: "row", gap: 10 },
  btnReset: {
    flex: 1,
    borderWidth: 1,
    borderColor: semantic.border,
    borderRadius: radii.full,
    paddingVertical: 12,
    alignItems: "center",
  },
  btnResetText: { fontSize: 14, fontWeight: "600", color: semantic.foreground },
  btnRun: {
    flex: 1,
    backgroundColor: colors.green600,
    borderRadius: radii.full,
    paddingVertical: 12,
    alignItems: "center",
  },
  btnRunText: { fontSize: 14, fontWeight: "700", color: semantic.foregroundWhite },
  btnDisabled: { opacity: 0.5 },
  successBanner: {
    fontSize: 15,
    fontWeight: "700",
    color: colors.green600,
    textAlign: "center",
    backgroundColor: colors.green50,
    padding: 12,
    borderRadius: radii.xl,
    overflow: "hidden",
  },
  resultEmpty: { fontSize: 13, color: semantic.foregroundMuted, textAlign: "center" },
  resultItem: {
    flexDirection: "row",
    alignItems: "flex-start",
    gap: 8,
    padding: 12,
    backgroundColor: semantic.surface,
    borderRadius: radii.lg,
    borderLeftWidth: 4,
  },
  resultIcon: { fontSize: 16 },
  resultDesc: { fontSize: 13, fontWeight: "600", color: semantic.foreground },
  resultExpected: { fontSize: 12, color: colors.green600 },
  resultGot: { fontSize: 12, color: colors.red400 },
});
