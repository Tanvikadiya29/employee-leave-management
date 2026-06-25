import axiosInstance from './axiosInstance';

export const getAllEmployees = (page = 1, pageSize = 10) =>
  axiosInstance.get(`/employees?page=${page}&pageSize=${pageSize}`);

export const getEmployeeById = (id) =>
  axiosInstance.get(`/employees/${id}`);

export const createEmployee = (data) =>
  axiosInstance.post('/employees', data);

export const updateEmployee = (id, data) =>
  axiosInstance.put(`/employees/${id}`, data);

export const deleteEmployee = (id) =>
  axiosInstance.delete(`/employees/${id}`);
