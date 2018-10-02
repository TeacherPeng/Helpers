using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PengSW.CommandPattern
{
    public class CommandPatternManager : INotifyPropertyChanged
    {
        public void Exec(ICommandPattern aCommand)
        {
            if (aCommand == null) return;
            aCommand.Do();
            _DoneCommands.Push(aCommand);
            _UndoCommands.Clear();
            OnPropertyChanged(nameof(CanUndo));
            OnPropertyChanged(nameof(CanRedo));
        }

        public bool CanUndo => _DoneCommands.Count > 0;

        public void Undo()
        {
            if (_DoneCommands.Count == 0) return;
            ICommandPattern aCommand = _DoneCommands.Pop();
            aCommand.Undo();
            _UndoCommands.Push(aCommand);
            OnPropertyChanged(nameof(CanUndo));
            OnPropertyChanged(nameof(CanRedo));
        }

        public bool CanRedo => _UndoCommands.Count > 0;

        public void Redo()
        {
            if (_UndoCommands.Count == 0) return;
            ICommandPattern aCommand = _UndoCommands.Pop();
            aCommand.Do();
            _DoneCommands.Push(aCommand);
            OnPropertyChanged(nameof(CanUndo));
            OnPropertyChanged(nameof(CanRedo));
        }

        public void Clear()
        {
            _DoneCommands.Clear();
            _UndoCommands.Clear();
            OnPropertyChanged(nameof(CanUndo));
            OnPropertyChanged(nameof(CanRedo));
        }

        protected Stack<ICommandPattern> _DoneCommands = new Stack<ICommandPattern>();
        protected Stack<ICommandPattern> _UndoCommands = new Stack<ICommandPattern>();

        protected void OnPropertyChanged(string aPropertyName) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(aPropertyName)); }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
