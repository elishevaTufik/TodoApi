import axios from 'axios';

axios.defaults.baseURL = process.env.REACT_APP_API_URL;

axios.interceptors.response.use(
  response => response,
  error => {
    console.error("API error:", error.response ? error.response.data : error.message);
    return Promise.reject(error);
  }
);

export default {

  getTasks: async () => {
    const result = await axios.get("/todos");
    return result.data;
  },

  addTask: async (name) => {
    try {
      const newTask = { name, isComplete: false };
      const result = await axios.post("/todos", newTask);
      return result.data;
    }
    catch (error) {
      console.error("Failed to add task:", error);
      throw error;
    }
  },

  setCompleted: async (id, isComplete) => {
    try {
      const result = await axios.put(`/todos/${id}`, { isComplete }); 
      return result.data; 
    } 
    catch (error) {
      console.error("Failed to update task completion:", error);
      throw error;
    }
  },

  deleteTask: async (id) => {
    try {
      await axios.delete(`/todos/${id}`);  
      return { success: true };  
    }
    catch (error) {
      console.error("Failed to delete task:", error);
      throw error; 
    }
  }
};
