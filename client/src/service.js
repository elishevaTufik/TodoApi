import axios from 'axios';

// הגדרת כתובת ה-API כברירת מחדל
axios.defaults.baseURL = "https://localhost:7271";  // שים לב לוודא שה-API רץ על ה-port הזה

// interceptor לשגיאות - תופס את השגיאות ומרשום ללוג
axios.interceptors.response.use(
  response => response,
  error => {
    console.error("API error:", error.response ? error.response.data : error.message);
    return Promise.reject(error);  // מחזיר את השגיאה
  }
);

export default {
  // פונקציה לשליפת כל המשימות
  getTasks: async () => {
    const result = await axios.get("/items");    
    return result.data;
  },

  // פונקציה להוספת משימה חדשה
  addTask: async (name) => {
    try {
      const newTask = { name, isComplete: false };  // המשימה החדשה עם שם וסטטוס ברירת מחדל
      const result = await axios.post("/items", newTask);  // שולח את הנתונים ל-API
      return result.data;  // מחזיר את התגובה מה-API
    } catch (error) {
      console.error("Failed to add task:", error);
      throw error;  // אם יש שגיאה, זורק אותה למעלה
    }
  },

  // פונקציה לעדכון סטטוס השלמה של משימה
  setCompleted: async (id, isComplete) => {
    try {
      const result = await axios.put(`/items/${id}`, { isComplete });  // שולח עדכון עבור המשימה
      return result.data;  // מחזיר את התגובה מה-API
    } catch (error) {
      console.error("Failed to update task completion:", error);
      throw error;  // אם יש שגיאה, זורק אותה למעלה
    }
  },

  // פונקציה למחיקת משימה
  deleteTask: async (id) => {
    try {
      await axios.delete(`/items/${id}`);  // שולח בקשה למחוק את המשימה
      return { success: true };  // מחזיר תשובה חיובית
    } catch (error) {
      console.error("Failed to delete task:", error);
      throw error;  // אם יש שגיאה, זורק אותה למעלה
    }
  }
};
