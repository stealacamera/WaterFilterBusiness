import { createSlice } from '@reduxjs/toolkit';
// utils
import axios from '../../utils/axios';
//
import { dispatch } from '../store';

// ----------------------------------------------------------------------

const initialState = {
  isLoading: false,
  error: null,
  events: [],
  isOpenModal: false,
  selectedScheduleId: null,
  selectedRange: null,
};

const slice = createSlice({
  name: 'calendar',
  initialState,
  reducers: {
    // START LOADING
    startLoading(state) {
      state.isLoading = true;
    },

    // HAS ERROR
    hasError(state, action) {
      state.isLoading = false;
      state.error = action.payload;
    },

    // GET EVENTS
    getSchedulesSuccess(state, action) {
      state.isLoading = false;
      state.events = action.payload;
    },

    // CREATE EVENT
    createScheduleSuccess(state, action) {
      const newSchedule = action.payload;
      state.isLoading = false;
      state.events = [...state.events, newSchedule];
    },

    // UPDATE EVENT
    updateScheduleSuccess(state, action) {
      const event = action.payload;
      const updateSchedule = state.events.map((_event) => {
        if (_event.id === event.id) {
          return event;
        }
        return _event;
      });

      state.isLoading = false;
      state.events = updateSchedule;
    },

    // DELETE EVENT
    deleteScheduleSuccess(state, action) {
      const { eventId } = action.payload;
      const deleteSchedule = state.events.filter((event) => event.id !== eventId);
      state.events = deleteSchedule;
    },

    // SELECT EVENT
    selectSchedule(state, action) {
      const eventId = action.payload;
      state.isOpenModal = true;
      state.selectedScheduleId = eventId;
    },

    // SELECT RANGE
    selectRange(state, action) {
      const { start, end } = action.payload;
      state.isOpenModal = true;
      state.selectedRange = { start, end };
    },

    // OPEN MODAL
    openModal(state) {
      state.isOpenModal = true;
    },

    // CLOSE MODAL
    closeModal(state) {
      state.isOpenModal = false;
      state.selectedScheduleId = null;
      state.selectedRange = null;
    },
  },
});

// Reducer
export default slice.reducer;

// Actions
export const { openModal, closeModal, selectSchedule } = slice.actions;

// ----------------------------------------------------------------------

const baseUrl = 'https://localhost:7117';
const userId = 4012;

const jwtToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI0MDEyIiwiZW1haWwiOiJ1c2VyMTIzQGV4YW1wbGUuY29tIiwiZXhwIjoxNzE2MjM1ODk1LCJpc3MiOiJodHRwczovL2xvY2FsaG9zdDo3MTE3LyIsImF1ZCI6Imh0dHBzOi8vbG9jYWxob3N0OjcxMTcvIn0.qaTE9_Eu763cIJH1LtzyDcEYDL5RkkS_3eHO-6nWyfw';
const generateAuthHeader = (jwtToken) => { return { headers: { 'Authorization': `Bearer ${jwtToken}` }}};

export function getSchedules() {

  return async () => {
    dispatch(slice.actions.startLoading());
    try {
      const response = await axios.get(
        `${baseUrl}/api/SalesAgentSchedules/salesAgents/${userId}`,
        generateAuthHeader(jwtToken));

      dispatch(slice.actions.getSchedulesSuccess(response.data));
    } catch (error) {
      dispatch(slice.actions.hasError(error));
    }
  };
}

// ----------------------------------------------------------------------

export function createSchedule(newSchedule) {
  return async () => {
    dispatch(slice.actions.startLoading());
    try {
      const response = await axios.post(
        `${baseUrl}/api/SalesAgentSchedules/salesAgents/${userId}`,
        newSchedule,
        generateAuthHeader(jwtToken));

      dispatch(slice.actions.createScheduleSuccess(response.data));
    } catch (error) {
      dispatch(slice.actions.hasError(error));
    }
  };
}

// ----------------------------------------------------------------------

export function updateSchedule(eventId, updateSchedule) {
  return async () => {
    dispatch(slice.actions.startLoading());
    try {
      const response = await axios.post('/api/calendar/events/update', {
        eventId,
        updateSchedule,
      });
      dispatch(slice.actions.updateScheduleSuccess(response.data.event));
    } catch (error) {
      dispatch(slice.actions.hasError(error));
    }
  };
}

// ----------------------------------------------------------------------

export function deleteSchedule(eventId) {
  return async () => {
    dispatch(slice.actions.startLoading());
    try {
      await axios.post('/api/calendar/events/delete', { eventId });
      dispatch(slice.actions.deleteScheduleSuccess({ eventId }));
    } catch (error) {
      dispatch(slice.actions.hasError(error));
    }
  };
}

// ----------------------------------------------------------------------

export function selectRange(start, end) {
  return async () => {
    dispatch(
      slice.actions.selectRange({
        start: start.getTime(),
        end: end.getTime(),
      })
    );
  };
}
