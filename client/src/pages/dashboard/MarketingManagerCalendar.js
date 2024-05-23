import FullCalendar from '@fullcalendar/react'; // => request placed at the top
import listPlugin from '@fullcalendar/list';
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import timelinePlugin from '@fullcalendar/timeline';
import interactionPlugin from '@fullcalendar/interaction';
//
import { useState, useRef, useEffect } from 'react';
// @mui
import { Card, Button, Container, DialogTitle, TextField, MenuItem, Stack } from '@mui/material';
// redux
import { useDispatch, useSelector } from '../../redux/store';
import { getSchedules, openModal, closeModal, updateSchedule, selectSchedule, selectRange } from '../../redux/slices/calendar';
// routes
import { PATH_DASHBOARD } from '../../routes/paths';
// hooks
import useSettings from '../../hooks/useSettings';
import useResponsive from '../../hooks/useResponsive';
// components
import Page from '../../components/Page';
import Iconify from '../../components/Iconify';
import { DialogAnimate } from '../../components/animate';
import HeaderBreadcrumbs from '../../components/HeaderBreadcrumbs';
// sections
import { CalendarForm, CalendarStyle, CalendarToolbar } from '../../sections/@dashboard/calendar';

// ----------------------------------------------------------------------

const selectedScheduleSelector = (state) => {
  const { events, selectedScheduleId } = state.calendar;
  if (selectedScheduleId) {
    return events.find((_event) => _event.id === selectedScheduleId);
  }
  return null;
};

export default function MarketingManagerCalendar() {
  const { themeStretch } = useSettings();

  const dispatch = useDispatch();

  const isDesktop = useResponsive('up', 'sm');

  const calendarRef = useRef(null);

  const [date, setDate] = useState(new Date());

  const [view, setView] = useState(isDesktop ? 'timeGridWeek' : 'listWeek');

  const selectedSchedule = useSelector(selectedScheduleSelector);

  const { events, isOpenModal, selectedRange } = useSelector((state) => state.calendar);

  const randomNames = ['John Doe', 'Jane Smith', 'Alex Johnson'];

  const [filterService, setFilterService] = useState('');

  const onFilterService = (event) => {
    setFilterService(event.target.value);
  };

  useEffect(() => {
    dispatch(getSchedules());
  }, [dispatch]);

  useEffect(() => {
    const calendarEl = calendarRef.current;
    if (calendarEl) {
      const calendarApi = calendarEl.getApi();
      const newView = isDesktop ? 'timeGridWeek' : 'listWeek';
      // const newView = 'timeGridWeek';
      calendarApi.changeView(newView);
      setView(newView);
    }
  }, [isDesktop]);

  const handleClickToday = () => {
    const calendarEl = calendarRef.current;
    if (calendarEl) {
      const calendarApi = calendarEl.getApi();
      calendarApi.today();
      setDate(calendarApi.getDate());
    }
  };

  const handleChangeView = (newView) => {
    const calendarEl = calendarRef.current;
    if (calendarEl) {
      const calendarApi = calendarEl.getApi();
      calendarApi.changeView(newView);
      console.log(newView);
      setView(newView);
    }
  };

  const handleClickDatePrev = () => {
    const calendarEl = calendarRef.current;
    if (calendarEl) {
      const calendarApi = calendarEl.getApi();
      calendarApi.prev();
      setDate(calendarApi.getDate());
    }
  };

  const handleClickDateNext = () => {
    const calendarEl = calendarRef.current;
    if (calendarEl) {
      const calendarApi = calendarEl.getApi();
      calendarApi.next();
      setDate(calendarApi.getDate());
    }
  };

  const handleSelectRange = (arg) => {
    const calendarEl = calendarRef.current;
    if (calendarEl) {
      const calendarApi = calendarEl.getApi();
      calendarApi.unselect();
    }
    dispatch(selectRange(arg.start, arg.end));
  };

  const handleSelectSchedule = (arg) => {
    dispatch(selectSchedule(arg.event.id));
  };

  const handleResizeSchedule = async ({ event }) => {
    try {
      dispatch(
        updateSchedule(event.id, {
          allDay: event.allDay,
          start: event.start,
          end: event.end,
        })
      );
    } catch (error) {
      console.error(error);
    }
  };

  const handleDropSchedule = async ({ event }) => {
    try {
      dispatch(
        updateSchedule(event.id, {
          allDay: event.allDay,
          start: event.start,
          end: event.end,
        })
      );
    } catch (error) {
      console.error(error);
    }
  };

  const handleAddSchedule = () => {
    dispatch(openModal());
  };

  const handleCloseModal = () => {
    dispatch(closeModal());
  };

  return (
    <Page title="Sales Agent Calendar">
      <Container maxWidth={themeStretch ? false : 'xl'}>
        <HeaderBreadcrumbs
          heading="Sales Agent Calendar"
          links={[{ name: 'Dashboard', href: PATH_DASHBOARD.root }, { name: 'Calendar' }]}
          action={
            <Stack spacing={2} direction={{ xs: 'column', md: 'row' }} sx={{ py: 2.5, px: 3 }}>
              <TextField
                fullWidth
                select
                label="Sales Agent"
                value={filterService}
                onChange={onFilterService}
                SelectProps={{
                  MenuProps: {
                    sx: { '& .MuiPaper-root': { maxHeight: 260 } },
                  },
                }}
                sx={{
                  maxWidth: { md: 200 },
                  minWidth: '150px',
                  textTransform: 'capitalize',
                }}
              >
                <MenuItem disabled value="">
                  <em>Sales Agent</em>
                </MenuItem>
                {randomNames.map((name, index) => (
                  <MenuItem
                    key={index}
                    value={name}
                    sx={{
                      mx: 1,
                      my: 0.5,
                      borderRadius: 0.75,
                      typography: 'body2',
                      textTransform: 'capitalize',
                    }}
                  >
                    {name}
                  </MenuItem>
                ))}
              </TextField>
              <Button
                variant="contained"
                startIcon={<Iconify icon={'eva:plus-fill'} width={20} height={20} />}
                onClick={handleAddSchedule}
                sx={{ maxWidth: '200px', minWidth: '150px' }}
              >
                New Schedule
              </Button>
            </Stack>
          }
        />

        <Card>
          <CalendarStyle>
            <CalendarToolbar
              date={date}
              view={view}
              onNextDate={handleClickDateNext}
              onPrevDate={handleClickDatePrev}
              onToday={handleClickToday}
              onChangeView={handleChangeView}
            />
            <FullCalendar
              weekends
              editable
              droppable
              selectable
              events={events}
              ref={calendarRef}
              rerenderDelay={10}
              initialDate={date}
              initialView={view}
              dayMaxScheduleRows={3}
              eventDisplay="block"
              headerToolbar={false}
              allDayMaintainDuration
              eventResizableFromStart
              select={handleSelectRange}
              eventDrop={handleDropSchedule}
              eventClick={handleSelectSchedule}
              eventResize={handleResizeSchedule}
              height={isDesktop ? 720 : 'auto'}
              plugins={[listPlugin, dayGridPlugin, timelinePlugin, timeGridPlugin, interactionPlugin]}
            />
          </CalendarStyle>
        </Card>

        <DialogAnimate open={isOpenModal} onClose={handleCloseModal}>
          <DialogTitle>{selectedSchedule ? 'Edit Schedule' : 'Add Schedule'}</DialogTitle>

          <CalendarForm event={selectedSchedule || {}} range={selectedRange} onCancel={handleCloseModal} />
        </DialogAnimate>
      </Container>
    </Page>
  );
}
