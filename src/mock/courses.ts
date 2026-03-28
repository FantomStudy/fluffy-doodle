export interface Course {
  image: string;
  category: string;
  title: string;
  description: string;
  level: "Начальный" | "Средний" | "Продвинутый";
  lessons: number;
}

export const COURSES: Course[] = [
  {
    image: "/assets/homepage/course-python.png",
    category: "Программирование",
    title: "Изучение Python",
    description: "Краткое описание про курс",
    level: "Средний",
    lessons: 12,
  },
  {
    image: "/assets/homepage/course-js.png",
    category: "Программирование",
    title: "Основы JavaScript",
    description: "Введение в веб-разработку",
    level: "Начальный",
    lessons: 10,
  },
  {
    image: "/assets/homepage/course-design.png",
    category: "Дизайн",
    title: "Дизайн интерфейсов",
    description: "Создание удобных интерфейсов",
    level: "Средний",
    lessons: 15,
  },
];
