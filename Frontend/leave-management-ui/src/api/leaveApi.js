import axiosInstance from './axiosInstance';

export const getAllLeaves = (page = 1, pageSize = 10, status = '') =>
  axiosInstance.get(`/leaverequests?page=${page}&pageSize=${pageSize}&status=${status}`);

export const getMyLeaves = (page = 1, pageSize = 10) =>
  axiosInstance.get(`/leaverequests/my?page=${page}&pageSize=${pageSize}`);

export const getLeaveById = (id) =>
  axiosInstance.get(`/leaverequests/${id}`);

export const createLeave = (data) =>
  axiosInstance.post('/leaverequests', data);

export const approveLeave = (id, data = {}) =>
  axiosInstance.put(`/leaverequests/${id}/approve`, data);

export const rejectLeave = (id, data = {}) =>
  axiosInstance.put(`/leaverequests/${id}/reject`, data);

export const cancelLeave = (id) =>
  axiosInstance.delete(`/leaverequests/${id}`);
