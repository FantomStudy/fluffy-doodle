import type { Task } from "../api/courses";
import * as React from "react";
import { useState } from "react";
import { Pressable, ScrollView, StyleSheet, Text, View } from "react-native";
import { colors, radii, semantic } from "../theme/tokens";

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

const THEORY_SLIDES = [
  {
    icon: "📦",
    title: "Переменная — это ящичек",
    text: "Переменная хранит данные: число, текст, список. Ты даёшь ей имя и кладёшь туда значение.",
    code: `let age = 12;\nconst name = "Алёша";\nvar score = 100;`,
  },
  {
    icon: "🔀",
    title: "Условие if / else",
    text: "Условие проверяет — правда или нет. Если правда — выполняется первый блок. Иначе — else.",
    code: `let score = 80;\nif (score >= 60) {\n  console.log("Сдал!");\n} else {\n  console.log("Попробуй снова");\n}`,
  },
  {
    icon: "🔁",
    title: "Цикл — это повторение",
    text: "Цикл выполняет блок кода много раз. Цикл for удобен, когда знаешь число повторений.",
    code: `for (let i = 1; i <= 3; i++) {\n  console.log("Шаг " + i);\n}`,
  },
  {
    icon: "🎯",
    title: "Готов к тесту?",
    text: "8 вопросов. Где-то нужно выбрать один правильный ответ, где-то — несколько. Удачи!",
  },
];

const QUESTIONS: Question[] = [
  {
    id: 1,
    type: "single",
    text: "Что такое переменная?",
    options: [
      "Функция, которая что-то вычисляет",
      "Ящичек для хранения данных",
      "Команда",
      "Способ остановить программу",
    ],
    correct: [1],
    explanation: "Переменная — это именованное место в памяти для хранения значения.",
  },
  {
    id: 2,
    type: "single",
    text: "Чему равно выражение?",
    code: "2 + 3 * 4",
    options: ["20", "14", "9", "24"],
    correct: [1],
    explanation: "Умножение раньше сложения: 3 × 4 = 12, 2 + 12 = 14.",
  },
  {
    id: 3,
    type: "multiple",
    text: "Выбери все ключевые слова для объявления переменных:",
    options: ["let", "const", "var", "function", "if"],
    correct: [0, 1, 2],
    explanation: "let, const и var объявляют переменные.",
  },
  {
    id: 4,
    type: "single",
    text: "Что выведет этот код?",
    code: `let x = 5;\nif (x > 10) {\n  console.log("Большое");\n} else {\n  console.log("Маленькое");\n}`,
    options: ['"Большое"', '"Маленькое"', "Ничего", "Ошибку"],
    correct: [1],
    explanation: '5 > 10 — ложь, выполняется else → "Маленькое".',
  },
  {
    id: 5,
    type: "multiple",
    text: "Какие значения являются «ложными» (falsy)?",
    options: ["0", '""', "null", "1", '"false"'],
    correct: [0, 1, 2],
    explanation: 'Falsy: 0, "", null, undefined, false, NaN.',
  },
  {
    id: 6,
    type: "single",
    text: "Сколько раз выполнится тело цикла?",
    code: `for (let i = 0; i < 4; i++) {\n  console.log(i);\n}`,
    options: ["3 раза", "4 раза", "5 раз", "0 раз"],
    correct: [1],
    explanation: "i: 0, 1, 2, 3 — 4 итерации.",
  },
  {
    id: 7,
    type: "multiple",
    text: "Что верно про const?",
    options: [
      "Нельзя переназначить",
      "Нужно сразу инициализировать",
      "Можно изменить позже",
      "Содержимое массива можно менять",
    ],
    correct: [0, 1, 3],
    explanation: "const нельзя переназначить, но содержимое объектов/массивов менять можно.",
  },
  {
    id: 8,
    type: "single",
    text: "Что делает return внутри функции?",
    options: [
      "Перезапускает функцию",
      "Возвращает значение",
      "Удаляет функцию",
      "Выводит в консоль",
    ],
    correct: [1],
    explanation: "return отправляет значение обратно и завершает функцию.",
  },
];

type OptionState = "idle" | "selected" | "correct" | "wrong" | "missed";

const getOptionState = (
  i: number,
  selected: Set<number>,
  correct: number[],
  checked: boolean,
): OptionState => {
  if (!checked) return selected.has(i) ? "selected" : "idle";
  const isCorrect = correct.includes(i);
  const isSelected = selected.has(i);
  if (isCorrect && isSelected) return "correct";
  if (isCorrect && !isSelected) return "missed";
  if (!isCorrect && isSelected) return "wrong";
  return "idle";
};

const optionColors: Record<OptionState, { bg: string; border: string; text: string }> = {
  idle: { bg: semantic.surface, border: semantic.border, text: semantic.foreground },
  selected: { bg: colors.purple50, border: colors.purple600, text: colors.purple600 },
  correct: { bg: colors.green50, border: colors.green600, text: colors.green600 },
  wrong: { bg: "#fef2f2", border: colors.red400, text: colors.red400 },
  missed: { bg: colors.amber50, border: colors.amber500, text: colors.amber500 },
};

const TheoryPhase = ({ onFinish }: { onFinish: () => void }) => {
  const [idx, setIdx] = useState(0);
  const slide = THEORY_SLIDES[idx];
  const isLast = idx === THEORY_SLIDES.length - 1;

  return (
    <View style={styles.theoryShell}>
      <View style={styles.progressBar}>
        <View
          style={[
            styles.progressFill,
            { width: `${((idx + 1) / THEORY_SLIDES.length) * 100}%` as any },
          ]}
        />
      </View>

      <ScrollView contentContainerStyle={styles.theoryBody}>
        <Text style={styles.theoryIcon}>{slide.icon}</Text>
        <Text style={styles.theoryStep}>
          Слайд {idx + 1} из {THEORY_SLIDES.length}
        </Text>
        <Text style={styles.theoryTitle}>{slide.title}</Text>
        <Text style={styles.theoryText}>{slide.text}</Text>
        {slide.code && (
          <View style={styles.codeBlock}>
            <Text style={styles.codeText}>{slide.code}</Text>
          </View>
        )}
      </ScrollView>

      <View style={styles.navRow}>
        {idx > 0 && (
          <Pressable style={styles.navBtn} onPress={() => setIdx(idx - 1)}>
            <Text style={styles.navBtnText}>Назад</Text>
          </Pressable>
        )}
        <View style={styles.dots}>
          {THEORY_SLIDES.map((_, i) => (
            <View key={i} style={[styles.dot, i === idx && styles.dotActive]} />
          ))}
        </View>
        {isLast ? (
          <Pressable style={[styles.navBtn, styles.navBtnStart]} onPress={onFinish}>
            <Text style={styles.navBtnStartText}>Начать тест 📝</Text>
          </Pressable>
        ) : (
          <Pressable style={[styles.navBtn, styles.navBtnNext]} onPress={() => setIdx(idx + 1)}>
            <Text style={styles.navBtnNextText}>Далее</Text>
          </Pressable>
        )}
      </View>
    </View>
  );
};

const ResultsScreen = ({
  correct,
  total,
  onRetry,
  onTheory,
}: {
  correct: number;
  total: number;
  onRetry: () => void;
  onTheory: () => void;
}) => {
  const pct = Math.round((correct / total) * 100);
  const xp = Math.round(pct / 10) * 5;
  const emoji = pct === 100 ? "🏆" : pct >= 75 ? "🎉" : pct >= 50 ? "👍" : "💪";
  const msg =
    pct === 100
      ? "Идеальный результат!"
      : pct >= 75
        ? "Отличная работа!"
        : pct >= 50
          ? "Неплохо!"
          : "Не сдавайся!";

  return (
    <ScrollView contentContainerStyle={styles.resultsShell}>
      <Text style={styles.resultsEmoji}>{emoji}</Text>
      <Text style={styles.resultsTitle}>Тест завершён!</Text>
      <Text style={styles.resultsScore}>
        {correct} / {total}
      </Text>
      <View style={styles.resultBar}>
        <View
          style={[
            styles.resultBarFill,
            {
              width: `${pct}%` as any,
              backgroundColor:
                pct >= 75 ? colors.green600 : pct >= 50 ? colors.amber500 : colors.red400,
            },
          ]}
        />
      </View>
      <Text style={styles.resultPct}>{pct}% правильных ответов</Text>
      <Text style={styles.resultsMsg}>{msg}</Text>
      <Text style={styles.xpBadge}>⭐ +{xp} XP</Text>
      <View style={styles.resultsBtns}>
        <Pressable style={styles.btnTheory} onPress={onTheory}>
          <Text style={styles.btnTheoryText}>📖 Повторить теорию</Text>
        </Pressable>
        <Pressable style={styles.btnRetry} onPress={onRetry}>
          <Text style={styles.btnRetryText}>🔄 Пройти снова</Text>
        </Pressable>
      </View>
    </ScrollView>
  );
};

const QuizPhase = ({
  onBackToTheory,
  onComplete,
}: {
  onBackToTheory: () => void;
  onComplete?: () => void;
}) => {
  const [qIdx, setQIdx] = useState(0);
  const [selected, setSelected] = useState<Set<number>>(new Set());
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
        next.has(i) ? next.delete(i) : next.add(i);
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
    <View style={styles.quizShell}>
      <View style={styles.quizHeader}>
        <Text style={styles.qCounter}>
          Вопрос {qIdx + 1} из {total}
        </Text>
        <View style={styles.progressBar}>
          <View style={[styles.progressFill, { width: `${((qIdx + 1) / total) * 100}%` as any }]} />
        </View>
        <Text style={styles.qScore}>✅ {correctCount}</Text>
      </View>

      <ScrollView contentContainerStyle={styles.quizBody}>
        <View style={styles.typeBadge}>
          <Text style={styles.typeBadgeText}>
            {q.type === "single" ? "Один правильный ответ" : "Несколько правильных ответов"}
          </Text>
        </View>
        <Text style={styles.qText}>{q.text}</Text>
        {q.code && (
          <View style={styles.codeBlock}>
            <Text style={styles.codeText}>{q.code}</Text>
          </View>
        )}
        <View style={styles.optionsList}>
          {q.options.map((opt, i) => {
            const state = getOptionState(i, selected, q.correct, checked);
            const c = optionColors[state];
            return (
              <Pressable
                key={i}
                style={[styles.option, { backgroundColor: c.bg, borderColor: c.border }]}
                onPress={() => toggle(i)}
                disabled={checked && state === "idle"}
              >
                <View style={[styles.optionMarker, { borderColor: c.border }]}>
                  {(state === "correct" || state === "selected") && (
                    <Text style={{ color: c.text }}>✓</Text>
                  )}
                  {state === "wrong" && <Text style={{ color: c.text }}>✗</Text>}
                  {state === "missed" && <Text style={{ color: c.text }}>○</Text>}
                </View>
                <Text style={[styles.optionText, { color: c.text }]}>{opt}</Text>
              </Pressable>
            );
          })}
        </View>
        {checked && (
          <View
            style={[
              styles.explanation,
              { backgroundColor: isAnswerCorrect ? colors.green50 : "#fef2f2" },
            ]}
          >
            <Text style={styles.explanationBadge}>
              {isAnswerCorrect ? "✅ Верно!" : "❌ Не совсем..."}
            </Text>
            <Text style={styles.explanationText}>{q.explanation}</Text>
          </View>
        )}
      </ScrollView>

      <View style={styles.quizFooter}>
        {!checked ? (
          <Pressable
            style={[styles.btnCheck, selected.size === 0 && styles.btnDisabled]}
            onPress={check}
            disabled={selected.size === 0}
          >
            <Text style={styles.btnCheckText}>✅ Проверить</Text>
          </Pressable>
        ) : (
          <Pressable style={styles.btnNext} onPress={next}>
            <Text style={styles.btnNextText}>
              {qIdx === total - 1 ? "Посмотреть результат 🏆" : "Следующий вопрос →"}
            </Text>
          </Pressable>
        )}
      </View>
    </View>
  );
};

const ApiQuizPhase = ({
  task,
  onComplete,
}: {
  task: Task;
  onComplete?: (ids: string[]) => void;
}) => {
  const [selected, setSelected] = useState<Set<string>>(new Set());
  const [submitted, setSubmitted] = useState(false);

  const toggle = (optionId: string) => {
    if (submitted) return;
    setSelected((prev) => {
      const next = new Set(prev);
      next.has(optionId) ? next.delete(optionId) : next.add(optionId);
      return next;
    });
  };

  const handleSubmit = () => {
    setSubmitted(true);
    onComplete?.(Array.from(selected));
  };

  return (
    <View style={styles.quizShell}>
      <View style={styles.quizHeader}>
        <Text style={styles.qCounter}>{task.title}</Text>
        <Text style={styles.qScore}>
          ⭐ {task.rewardStars} | +{task.rewardExp} XP
        </Text>
      </View>
      <ScrollView contentContainerStyle={styles.quizBody}>
        <View style={styles.typeBadge}>
          <Text style={styles.typeBadgeText}>Выбери правильные ответы</Text>
        </View>
        <Text style={styles.qText}>{task.question}</Text>
        <View style={styles.optionsList}>
          {task.options?.map((opt) => {
            const isSelected = selected.has(opt.id);
            return (
              <Pressable
                key={opt.id}
                style={[
                  styles.option,
                  {
                    backgroundColor: isSelected ? colors.purple50 : semantic.surface,
                    borderColor: isSelected ? colors.purple600 : semantic.border,
                  },
                ]}
                onPress={() => toggle(opt.id)}
                disabled={submitted}
              >
                <View
                  style={[
                    styles.optionMarker,
                    { borderColor: isSelected ? colors.purple600 : semantic.border },
                  ]}
                >
                  {isSelected && <Text style={{ color: colors.purple600 }}>✓</Text>}
                </View>
                <Text
                  style={[
                    styles.optionText,
                    { color: isSelected ? colors.purple600 : semantic.foreground },
                  ]}
                >
                  {opt.text}
                </Text>
              </Pressable>
            );
          })}
        </View>
        {submitted && (
          <View style={[styles.explanation, { backgroundColor: colors.green50 }]}>
            <Text style={styles.explanationBadge}>✅ Ответ отправлен!</Text>
            <Text style={styles.explanationText}>Твой результат обрабатывается</Text>
          </View>
        )}
      </ScrollView>
      <View style={styles.quizFooter}>
        {!submitted && (
          <Pressable
            style={[styles.btnCheck, selected.size === 0 && styles.btnDisabled]}
            onPress={handleSubmit}
            disabled={selected.size === 0}
          >
            <Text style={styles.btnCheckText}>✅ Отправить ответ</Text>
          </Pressable>
        )}
      </View>
    </View>
  );
};

interface QuizChallengeProps {
  task?: Task;
  onComplete?: (selectedOptionIds?: string[]) => void;
}

export const QuizChallenge = ({ task, onComplete }: QuizChallengeProps) => {
  const [phase, setPhase] = useState<"theory" | "quiz">("theory");

  if (task) {
    return phase === "theory" ? (
      <TheoryPhase onFinish={() => setPhase("quiz")} />
    ) : (
      <ApiQuizPhase task={task} onComplete={onComplete} />
    );
  }

  return phase === "theory" ? (
    <TheoryPhase onFinish={() => setPhase("quiz")} />
  ) : (
    <QuizPhase onBackToTheory={() => setPhase("theory")} onComplete={onComplete} />
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
  theoryBody: { flex: 1, padding: 24, alignItems: "center", gap: 12 },
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
    marginTop: 8,
  },
  codeText: { fontFamily: "monospace", fontSize: 13, color: "#c8d3f5", lineHeight: 20 },
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
  quizShell: { flex: 1, backgroundColor: semantic.background },
  quizHeader: {
    padding: 16,
    gap: 8,
    flexDirection: "row",
    alignItems: "center",
    justifyContent: "space-between",
  },
  qCounter: { fontSize: 14, fontWeight: "600", color: semantic.foreground },
  qScore: { fontSize: 13, fontWeight: "600", color: colors.green600 },
  quizBody: { padding: 16, gap: 16 },
  typeBadge: {
    alignSelf: "flex-start",
    backgroundColor: colors.purple50,
    paddingHorizontal: 12,
    paddingVertical: 4,
    borderRadius: radii.full,
  },
  typeBadgeText: { fontSize: 12, fontWeight: "600", color: colors.purple600 },
  qText: { fontSize: 18, fontWeight: "600", color: semantic.foreground, lineHeight: 26 },
  optionsList: { gap: 10 },
  option: {
    flexDirection: "row",
    alignItems: "center",
    gap: 12,
    borderWidth: 2,
    borderRadius: radii.xl,
    padding: 14,
  },
  optionMarker: {
    width: 24,
    height: 24,
    borderRadius: 12,
    borderWidth: 2,
    alignItems: "center",
    justifyContent: "center",
  },
  optionText: { fontSize: 15, flex: 1 },
  explanation: { padding: 16, borderRadius: radii.xl, gap: 6 },
  explanationBadge: { fontSize: 14, fontWeight: "700" },
  explanationText: { fontSize: 14, color: semantic.foregroundStrong, lineHeight: 20 },
  quizFooter: { padding: 16 },
  btnCheck: {
    backgroundColor: colors.purple600,
    borderRadius: radii.full,
    paddingVertical: 14,
    alignItems: "center",
  },
  btnCheckText: { fontSize: 15, fontWeight: "700", color: semantic.foregroundWhite },
  btnNext: {
    backgroundColor: colors.green600,
    borderRadius: radii.full,
    paddingVertical: 14,
    alignItems: "center",
  },
  btnNextText: { fontSize: 15, fontWeight: "700", color: semantic.foregroundWhite },
  btnDisabled: { opacity: 0.5 },
  resultsShell: { flex: 1, padding: 24, alignItems: "center", justifyContent: "center", gap: 16 },
  resultsEmoji: { fontSize: 64 },
  resultsTitle: { fontSize: 28, fontWeight: "700", color: semantic.foreground },
  resultsScore: { fontSize: 36, fontWeight: "800", color: semantic.primary },
  resultBar: {
    width: "100%",
    height: 12,
    backgroundColor: semantic.borderSubtle,
    borderRadius: radii.full,
    overflow: "hidden",
  },
  resultBarFill: { height: "100%", borderRadius: radii.full },
  resultPct: { fontSize: 14, fontWeight: "600", color: semantic.foregroundMuted },
  resultsMsg: { fontSize: 16, color: semantic.foreground, textAlign: "center" },
  xpBadge: {
    fontSize: 18,
    fontWeight: "700",
    color: colors.amber500,
    backgroundColor: colors.amber50,
    paddingHorizontal: 16,
    paddingVertical: 8,
    borderRadius: radii.full,
    overflow: "hidden",
  },
  resultsBtns: { flexDirection: "row", gap: 12, marginTop: 8 },
  btnTheory: {
    borderWidth: 2,
    borderColor: semantic.border,
    borderRadius: radii.full,
    paddingHorizontal: 16,
    paddingVertical: 10,
  },
  btnTheoryText: { fontSize: 14, fontWeight: "600", color: semantic.foreground },
  btnRetry: {
    backgroundColor: colors.purple600,
    borderRadius: radii.full,
    paddingHorizontal: 16,
    paddingVertical: 10,
  },
  btnRetryText: { fontSize: 14, fontWeight: "600", color: semantic.foregroundWhite },
});
