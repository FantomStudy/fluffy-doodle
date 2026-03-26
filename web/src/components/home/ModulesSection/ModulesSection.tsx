import type {ModuleCardProps} from "../ModuleCard";
import { ModuleCard  } from "../ModuleCard";
import s from "./ModulesSection.module.css";

const modules: ModuleCardProps[] = [
  {
    title: "Основы программирования",
    description: "Базовые команды, циклы и логика действий.",
    headerGradient:
      "linear-gradient(180deg, #6fc4ff 0%, #5f8eff 100%)",
    btnGradient: "linear-gradient(180deg, #6cbcff 0%, #438df6 100%)",
    btnShadow: "0px 10px 18px 0px rgba(71, 132, 238, 0.2)",
    progressLabel: "35% завершено",
    actionLabel: "Продолжить",
  },
  {
    title: "Логика и алгоритмы",
    description: "Учимся находить порядок действий через мини-игры.",
    headerGradient:
      "linear-gradient(180deg, #ffcd64 0%, #ff962f 100%)",
    btnGradient: "linear-gradient(180deg, #ffb44e 0%, #ff8420 100%)",
    btnShadow: "0px 10px 18px 0px rgba(71, 132, 238, 0.2)",
    progressLabel: "29% завершено",
    actionLabel: "Открыть",
  },
  {
    title: "Переменные",
    description: "Понимаем, как хранятся значения и зачем они нужны.",
    headerGradient:
      "linear-gradient(180deg, #9de5ef 0%, #7bc4f8 100%)",
    btnGradient: "linear-gradient(180deg, #7fd9ec 0%, #4c9de0 100%)",
    btnShadow: "0px 10px 18px 0px rgba(71, 132, 238, 0.2)",
    progressLabel: "Новый модуль",
    actionLabel: "Посмотреть",
    headerLetter: "C",
  },
];

export function ModulesSection() {
  return (
    <div className={s.section}>
      <div className={s.header}>
        <div className={s.titleGroup}>
          <span className={s.label}>Каталог модулей</span>
          <h3 className={s.title}>Учебные блоки по интересам</h3>
        </div>
        <p className={s.subtitle}>
          Подобраны так, чтобы ребенок видел прогресс и следующий шаг
        </p>
      </div>
      <div className={s.grid}>
        {modules.map((mod) => (
          <ModuleCard key={mod.title} {...mod} />
        ))}
      </div>
    </div>
  );
}
